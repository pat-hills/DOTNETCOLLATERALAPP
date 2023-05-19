using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model;
using CRL.Model.Configuration;
using CRL.Model.Messaging;
using CRL.Model.ModelService;
using CRL.Model.ModelService.FS;
using CRL.Model.Search;
using CRL.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using MicrosoftReportGenerators;
using CRL.Service.Interfaces;
using CRL.Model.FS;
using CRL.Model.Payments;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.Search;
using System.Net.Http;
using CRL.Service.Messaging;
using CRL.Model.Notification;
using System.Configuration;
namespace CRL.Service.Implementations
{
    public class SearchService : ServiceBase, ISearchService
    {
        string UniqueId { get; set; }
        public SearchService() : base() { }

        public ResponseBase VerifyPublicUser(VerifyPublicUserRequest request)
        {
            ResponseBase response = new ResponseBase();
            PublicUserSecurityCode publicUser = null;

            publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
            if (publicUser == null)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "The public security user code is invalid or expired!";
            }

            return response;
        }

        public GetSearchResponse GetSearchQ(GetSearchRequest request)
        {
            GetSearchResponse response = new GetSearchResponse();
            response.BusinessPrefixes = LookUpServiceModel.BusinessRegPrefixes(_registrationPrefixRepository);
            return response;
        }

        public GetSearchResponse GetSearch(GetSearchRequest request)
        {
            GetSearchResponse response = new GetSearchResponse();
            PublicUserSecurityCode publicUser = null;

            //If we have a security code verify that it exists and the security code is not expired
            if (request.SecurityUser == null && request.SecurityCodeForPublicUser != null)
            {
                publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
                if (publicUser == null)
                {
                    response.MessageInfo.MessageType = MessageType.Error;
                    response.MessageInfo.Message = "The PIN Code is invalid or expired!";
                }

                return response;
            }

            //Check if payment is involved for search

            TransactionPaymentSetup configurationFee = null;
            var LenderTypeId = 0;
            if (request.SecurityUser == null)
            {
                configurationFee = _configurationFeeRepository.GetDbSetComplete(ServiceFees.PublicSearch, Constants.DefaultLenderTypeIdForPublicUsers).FirstOrDefault();
            }
            else
            {
                LenderTypeId = (int)_institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
                configurationFee = _configurationFeeRepository.GetDbSetComplete(ServiceFees.Search, LenderTypeId).FirstOrDefault();
            }




            if (configurationFee != null && configurationFee.Fee > 0)
            {
                response.IsPayable = true;
            }



            response.BusinessPrefixes = LookUpServiceModel.BusinessRegPrefixes(_registrationPrefixRepository);


            return response;
        }

        public SearchResponse FilterOrLoadSearch(SearchRequest request)
        {
            //For now we are only support pagination but no filter
            SearchResponse response = new SearchResponse();

            response.BusinessPrefixes = LookUpServiceModel.BusinessRegPrefixes(_registrationPrefixRepository);
            SearchFinancialStatement searchFS = _searchFinancialStatementRepository.FindBy(request.Id);
            string[] FilteredRegistrationNo = SearchParameterHelper.GetObjectFromXML<string[]>(searchFS.FoundRegistrationXML);

            request.SearchParam = SearchParameterHelper.GetObjectFromXML<SearchParam>(searchFS.SearchParamXML);
            var reqtrieveFSQuery = _financialStatementRepository.GetDbSetComplete().Where(s => s.IsActive == true && FilteredRegistrationNo.Contains(s.RegistrationNo) && s.AfterUpdateFinancialStatementId == null);
            reqtrieveFSQuery = reqtrieveFSQuery.OrderBy(s => s.Id);  //**Let's do proper sorting later
            if (request.PageIndex > 0)
                reqtrieveFSQuery = reqtrieveFSQuery.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            response.SearchResultView = _financialStatementRepository.FindBy(reqtrieveFSQuery).ToList().ConvertToSearchResultView(request, _participantRepository, _collateralRepository);
            foreach (var result in response.SearchResultView)
            {
                result.EncryptedId = Util.Encrypt(result.Id.ToString());
            }
            if (searchFS.CACResultsXML != null)
            {
                response.ResultsFromCAC = SearchParameterHelper.GetObjectFromXML<CACSearch>(searchFS.CACResultsXML);
            }
            response.SearchParam = request.SearchParam;
            response.SearchDate = searchFS.CreatedOn;
            //response.AssignedFSId = searchFS.SelectedFSId;
            response.HasSearchReport = searchFS.GeneratedReportId == null ? false : true;

            int TotalRecords = FilteredRegistrationNo.Count();
            response.NumRecords = TotalRecords;
            response.Id = request.Id;

            return response;
        }

        public SearchResponse Search(SearchRequest request)
        {
            SearchResponse response = new SearchResponse();
            PaymentServiceModel paymentServiceModel = new PaymentServiceModel(_configurationFeeRepository, _membershipRepository, _currencyRepository, _accountTransactionRepository, auditTracker, request.SecurityUser);
            SearchServiceModel searchServiceModel = new SearchServiceModel(_serialTrackerRepository, _emailRepository, _emailTemplateRepository, _emailUserAssignmentRepository,
                _financialStatementTransactionCategoryRepository, _collateralCategoryRepository, _searchFinancialStatementRepository,
                _financialStatementRepository, _participantRepository, _collateralRepository,
                _financialStatementActivityRepository, _institutionRepository, _userRepository, auditTracker, request.SecurityUser, request.UniqueGuidForm);
            ServiceRequest serRequest = null;
            PublicUserSecurityCode publicUser = null;
            CACSearch searchResult = null;
            #region Validation
            //For institution user who is not owner allow only the search role to search
            if (Constants.ApplySearchRoleToSearch && request.SecurityUser != null && request.SecurityUser.InstitutionId != null)
            {
                if (!request.SecurityUser.IsInRole("Search"))
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }

            }
            #endregion

            #region Load Necessary Data
            //Loads an associated service request if we passed a unique identifier.  This will be validated to check if this
            //request has not already been handled
            if (String.IsNullOrWhiteSpace(request.UniqueGuidForm) == false)
                serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();
            if (request.SecurityUser == null && request.SecurityCodeForPublicUser != null)
            {
                publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
                if (publicUser == null)
                {
                    response.MessageInfo.MessageType = MessageType.Error;
                    response.MessageInfo.Message = "The PIN Code is invalid or expired!";
                    return response;
                }

            }
            var LenderTypeId = 0;
            //Loads and initialises payment related data
            if (request.SecurityUser == null)
            {
                paymentServiceModel.InitialisePayment(ServiceFees.PublicSearch, Constants.DefaultLenderTypeIdForPublicUsers);
            }
            else
            {
                LenderTypeId = (int)_institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
                paymentServiceModel.InitialisePayment(ServiceFees.Search, (int)LenderTypeId);
            }


            //Use the change service model to perform the appropirate loads
            searchServiceModel.LoadInitialDataFromRepository(0);
            #endregion

            #region Validation After Data Load (Unique Code, Payment)
            //When we re search we need to reset again first before we can do that
            if (this.ValidateUniqueServiceRequest(request.UniqueGuidForm, serRequest) == false)
            {
                response.SetResponseToResponseBase(this.ReturnAlreadyRequestedResponse()); return response;
            }

            PaymentInfoResponse paymentValidationResponse = null;

            if (request.IsPayable)
            {
                //Payment validation
                paymentValidationResponse = paymentServiceModel.ValidatePayment(null, publicUser);
                if (paymentValidationResponse.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(paymentValidationResponse); return response; }
            }
            #endregion

            if (request.SearchParam.SearchType == 2)
            {
                request.apiUrl = ConfigurationManager.AppSettings["cacurl"] + request.SearchParam.BorrowerIDNo + "&clas=" + request.clas + "&usr=ncr&pwd=Password1$";
                response.CACResults = ConnectToAPI.Connect(request.apiUrl);
                if (response.CACResults != "Error connectiong to CAC")
                {
                    searchResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CACSearch>(response.CACResults);
                }
                else
                {
                    searchResult = null;
                }

            }
            else
            {
                response.CACResults = null;
            }
            SearchFinancialStatement sch = searchServiceModel.Search(request, response);
            if (request.SearchParam.SearchType == 2)
            {
                sch.CACResultsXML = SearchParameterHelper.GenerateXML<CACSearch>(searchResult);
            }


            //You need to rework on this one again 
            short IsSearchOrGenerateResult = 1;
            if (request.IsPayable)
            {
                if (paymentValidationResponse != null && paymentValidationResponse.isPayabale == true)
                {
                    paymentServiceModel.ProcessSearchPayment(sch, IsSearchOrGenerateResult, publicUser);
                }
            }

            if (request.IsNonLegalEffectSearch)
            {

            }
            else
            {
                AuditAction = Model.FS.Enums.AuditAction.LegalEffectiveSearch;

            }
            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);




            #region auditing and committing
            //Audititng Processing
            AuditAction = Model.FS.Enums.AuditAction.FlexibleEffectiveSearch; //You need to reqork on this one again
            searchServiceModel.AssignSearchNo();  //**Configure to allow immediate saving to the serial tracker
            AuditMessage = "Search No: " + sch.SearchCode;
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            
            #endregion

            response.Id = sch.Id;
            response.FilterSearch.SearchId = sch.Id;


            //Updating payment record
            if (publicUser != null)
            {
                PublicUserSecurityCode publicUserSecurityCode = _publicUserSecurityCodeRepository.GetDbSet().SingleOrDefault(s => s.SecurityCode == request.SecurityCodeForPublicUser);
                if (publicUserSecurityCode != null)
                {
                    Payment payment = _paymentRepository.GetPaymentBySecurityCode(publicUserSecurityCode.Id);

                    if (payment.PaymentSource == PaymentSource.InterswitchDirectPay)
                    {
                        var interSwitchDirectPay = payment.InterSwitchDirectPayTransaction;
                        if (String.IsNullOrWhiteSpace(interSwitchDirectPay.BVN) || String.IsNullOrWhiteSpace(interSwitchDirectPay.CustPhone))
                        {
                            interSwitchDirectPay.CustName = request.PublicUserName;
                            interSwitchDirectPay.CustPhone = request.PublicUserBVN;
                            interSwitchDirectPay.BVN = request.PublicUserBVN;
                            auditTracker.Updated.Add(interSwitchDirectPay);

                            if (String.IsNullOrEmpty(publicUser.PublicUserEmail))
                            {
                                if (payment != null)
                                {

                                    publicUser.PublicUserEmail = request.PublicUserEmail;
                                    auditTracker.Updated.Add(publicUser);

                                    payment.Payee = request.PublicUserName;
                                    payment.PublicUserSecurityCode = publicUser;
                                    auditTracker.Updated.Add(payment);

                                }
                            }
                        }
                    }
                    else if (payment.PaymentSource == PaymentSource.InterSwitchWebPay)
                    {
                        var interSwitchWebPay = payment.InterSwitchWebPayTransaction;
                        if (String.IsNullOrWhiteSpace(interSwitchWebPay.BVN))
                        {
                            interSwitchWebPay.BVN = request.PublicUserBVN;
                        }
                    }

                }
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
                try
                {

                    _uow.Commit();

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return response;

        }

        /// <summary>
        /// Load the search request
        /// Replace the last generated report from this request
        /// Add the 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GenerateSearchReportResponse GenerateSearchReport(GenerateSearchReportRequest request)  //*Change to searchresponse rather
        {
            //We will need to check that the user who performed this searched is the same user who actually
            GenerateSearchReportResponse response = new GenerateSearchReportResponse();
            PaymentServiceModel paymentServiceModel = new PaymentServiceModel(_configurationFeeRepository, _membershipRepository, _currencyRepository, _accountTransactionRepository, auditTracker, request.SecurityUser);
            SearchServiceModel searchServiceModel = new SearchServiceModel(_serialTrackerRepository, _emailRepository, _emailTemplateRepository, _emailUserAssignmentRepository,
                _financialStatementTransactionCategoryRepository, _collateralCategoryRepository, _searchFinancialStatementRepository,
                _financialStatementRepository, _participantRepository, _collateralRepository,
                _financialStatementActivityRepository, _institutionRepository, _userRepository, auditTracker, request.SecurityUser, request.UniqueGuidForm);
            PublicUserSecurityCode publicUser = null;
            FSServiceModelBase.ReportGeneratedDate = AuditedDate;


            #region Validation
            //For institution user who is not owner allow only the search role to search
            if (Constants.ApplyGenerateSearchReportRole && request.SecurityUser != null && request.SecurityUser.InstitutionId != null)
            {
                if (!request.SecurityUser.IsInRole("Search"))
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }

            }
            #endregion

            #region Email Validation Checked And Not Provided!!

           // if((request.SendAsMail) && (request.publicUsrEmail == null)){

             //   response.MessageInfo.MessageType = MessageType.Error;
              //  response.MessageInfo.Message = "EmailNotNull!";
              //  return response;

          //  }
            #endregion


            #region Load Necessary Data
            //Loads and initialises payment related data
            var LenderTypeId = 0;
            if (request.SecurityUser == null)
            {
                paymentServiceModel.InitialisePayment(ServiceFees.CertifiedSearchResultPublic, Constants.DefaultLenderTypeIdForPublicUsers);
            }
            else
            {
                LenderTypeId = (int)_institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
                paymentServiceModel.InitialisePayment(ServiceFees.CertifiedSearchResult, LenderTypeId);
            }


            //Use the change service model to perform the appropirate loads
            searchServiceModel.LoadInitialDataFromRepository(request.Id);

            if (request.SecurityUser == null && request.SecurityCodeForPublicUser != null)
            {

                publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
                if (publicUser == null)
                {
                    response.MessageInfo.MessageType = MessageType.Error;
                    response.MessageInfo.Message = "The PIN Code is invalid or expired!";
                    return response;
                }
                request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);

            }
            #endregion

            #region Validation After Data Load (Unique Code, Payment)
            //Payment validation
            PaymentInfoResponse paymentValidationResponse = null;
            if (request.PayableTransaction || request.SecurityUser != null)
            {
                paymentValidationResponse = paymentServiceModel.ValidatePayment(null, publicUser);
                if (paymentValidationResponse.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(paymentValidationResponse); return response; }
            }
            #endregion
            List<FinancialStatement> financialStatements = null;
            if (searchServiceModel.fsSch.NumRecords > 0)
            {
                financialStatements = _financialStatementRepository.GetDbSet().Where(s => s.Id == request.SelectedFS).ToList();
            }
            else
            {
                financialStatements = new List<FinancialStatement>();
            }
            MFSSearchReportBuilder reportBuilder = new MFSSearchReportBuilder();
            searchServiceModel.GenerateSearchReport(reportBuilder, financialStatements, this.GetLookUpsForFs());  //**We need response to get error messages

            if (paymentValidationResponse != null && paymentValidationResponse.isPayabale == true)
            {
                paymentServiceModel.ProcessSearchPayment(searchServiceModel.fsSch, 2, publicUser);
            }



            //if (request.SecurityUser == null)
            //{
            //    request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            //    request.IsPublicUser = true;
            //}

            if (request.SendAsMail)
            {
                //Generate email
                List<Email> Emails = new List<Email>();
                Email email = new Email();
               // if (request.IsPublicUser)
                if (request.publicUsrEmail !=null )
                {
                    email.EmailTo = request.publicUsrEmail.Replace(',', ';');
                }
                else
                {
                    email.EmailTo = request.SecurityUser.Email;
                }
                //email.EmailTo = request.SecurityUser.Email;
                email.IsSent = false;
                email.NumRetries = 0;


                //Generate template
                EmailTemplate emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "SearchResponseMail").SingleOrDefault();
                EmailTemplateGenerator.SearchResponse(email, emailTemplate, searchServiceModel.fsSch.GeneratedReport);
                EmailUserAssignment emailAssignment = new EmailUserAssignment();
                //emailAssignment.UserId = request.SecurityUser.Id;
                emailAssignment.UserId = request.SecurityUser == null ? Constants.PublicUser : request.SecurityUser.Id;
                emailAssignment.IsActive = true;
                emailAssignment.IsRead = false;
                emailAssignment.Email = email;
                _emailRepository.Add(email);
                _emailUserAssignmentRepository.Add(emailAssignment);
                auditTracker.Created.Add(email);
                auditTracker.Created.Add(emailAssignment);
                response.EmailSent = true;

         
            }

            #region auditing and committing
            //Audititng Processing
            AuditAction = Model.FS.Enums.AuditAction.LegalEffectiveSearch;


            AuditMessage = "Search No: " + searchServiceModel.fsSch.SearchCode;
            var responseAfterCommit = this.AuditCommit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser == null ? Constants.PublicUser : request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }

            #endregion


            //response.AttachedFile = _fileUpload.AttachedFile;
            //response.AttachedFileName = _fileUpload.AttachedFileName;
            //response.AttachedFileSize = _fileUpload.AttachedFileSize;
            //response.AttachedFileType = _fileUpload.AttachedFileType;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;

            return response;

            //    PublicUserSecurityCode publicUser = null;
            //    //Get the audited date
            //    SearchFinancialStatement searchFS = _searchFinancialStatementRepository.FindBy(request.Id);
            //    if ( request.CheckUniqueIdentifier && (searchFS.UniqueIdentifier != request.UniqueIdentifier))
            //    {
            //        response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
            //        return response;
            //    }
            //    response.SearchId = searchFS.Id;


            //    if (!request.IsPublicUser)
            //    {
            //        //Check the role for only certified searches for logged in user and non individuals
            //        if (request.isCertified)
            //        {
            //            if (request.SecurityUser.IsOwnerUser == false && request.SecurityUser.InstitutionId != null &&
            //                !request.SecurityUser.IsInRole("Obtain Certified Search"))
            //            {
            //                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
            //                return response;
            //            }
            //        }

            //        if (searchFS.CreatedBy  != request.SecurityUser.Id)
            //        {
            //            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
            //            return response;
            //        }

            //        //check the 
            //    }
            //    else
            //    {
            //        if (request.isCertified)
            //        {
            //            if (!String.IsNullOrWhiteSpace(request.publicUserCode))
            //            {
            //                publicUser = _publicUserRepository.GetDbSet().Where(s => s.SecurityCode == request.publicUserCode && s.IsActive == true).SingleOrDefault();
            //                if (publicUser == null)
            //                {
            //                    throw new Exception("Unexpected Error when retreiving public user code to generate search report!");

            //                }

            //                //**If they expire then check for then return an apporirate erro for this to the user
            //            }
            //            else if (!String.IsNullOrWhiteSpace(request.publicUsrReceiptNo))
            //            {
            //                publicUser = _publicUserRepository.GetDbSet().Where(s => s.SecurityCode  == request.publicUsrReceiptNo && s.IsActive == true).SingleOrDefault(); ;
            //                if (publicUser == null)
            //                {
            //                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo
            //                    {
            //                        MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
            //                        Message = "Your receipt no is not valid!"
            //                    };
            //                    return response;
            //                }

            //                response.PublicUserReceiptNo = request.publicUsrReceiptNo;
            //            }
            //            else
            //            {
            //                response.MessageInfo = new Infrastructure.Messaging.MessageInfo
            //                {
            //                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
            //                    Message = "Please enter a valid receipt no!"
            //                };
            //                return response;
            //            }

            //        }
            //        //check that we have the oublic user code or the public receipt nofor public users
            //    }



            //   List<FSReportView> FS = new List<FSReportView>();
            // List<SearchParamView> SearchParam = new List<SearchParamView>();
            ////   List<FSReportView> UpdateFS = new List<FSReportView>();
            //   List<SecuredPartyReportView> Lenders = new List<SecuredPartyReportView>();
            //   List<DebtorReportView> Borrowers = new List<DebtorReportView>();
            //   List<CollateralReportView> Collaterals = new List<CollateralReportView>();
            //   List<UpdateActivityReportView> Updates = new List<UpdateActivityReportView>();
            //   List<DischargeActivityReportView> DischargeActivity = new List<DischargeActivityReportView>();
            //   List<AssignmentActivityReportView> AssignmentActivity = new List<AssignmentActivityReportView>();
            //   List<SubordinationActivityReportView> SubordinationActivity = new List<SubordinationActivityReportView>();
            //   List<ClientReportView> AssignedParty = new List<ClientReportView>();
            //   List<SubordinatingPartyReportView> SubordinatingParty = new List<SubordinatingPartyReportView>();
            //   List<CollateralReportView> DischargedCollaterals = new List<CollateralReportView>();
            //   List<ChangeDescriptionView> ChangeDescription = new List<Views.FinancialStatement.ChangeDescriptionView>();
            //   List<OtherIdentificationReportView> OtherIdentifications = new List<OtherIdentificationReportView>();
            //    List<FSActivitySummaryReportView> Activities = new List<FSActivitySummaryReportView>();

            //  //Generate search parameter

            //    SearchParam _legalSearchParam = SearchParameterHelper.GetObjectFromXML<SearchParam>(searchFS.SearchParamXML);
            //    SearchParamView _searchParamView = new SearchParamView
            //    {
            //         BorrowerFirstName = _legalSearchParam .BorrowerFirstName ,
            //          BorrowerIDNo= _legalSearchParam .BorrowerIDNo ,                     
            //             BorrowerType = _legalSearchParam .BorrowerIsIndividual==1 ? "Individual":"Financial Institution" ,
            //             BorrowerLastName = _legalSearchParam .BorrowerLastName ,
            //              BorrowerMiddleName = _legalSearchParam .BorrowerMiddleName ,
            //               BorrowerName =_legalSearchParam .BorrowerName ,
            //                CollateralDescription = _legalSearchParam .CollateralDescription ,
            //                 CollateralSerialNo =_legalSearchParam .CollateralSerialNo ,                              
            //                   IsNonLegalEffectSearch = searchFS .IsLegalOrFlexible ==1?"Legally Effective Search":"Flexible Search",
            //                    DebtorEmail = _legalSearchParam .DebtorEmail ,
            //                    SearchCode=searchFS.SearchCode ,
            //                     ReportType =request .isCertified ==true? "Certified Search Result": "Uncertified Search Result",
            //                      DateOfSearch = searchFS.CreatedOn  

            //    };

            //    if (_legalSearchParam.DebtorDateOfBirth != null)
            //    {
            //        _searchParamView.DebtorDateOfBirth = _legalSearchParam.DebtorDateOfBirth.StartDate;
            //    }

            //    SearchParam.Add(_searchParamView);
            //    if (request.SelectedFS != null)
            //    {
            //        var financialStatements = _financialStatementRepository.GetDbSet().Where(s => request.SelectedFS.Contains(s.Id)).ToList();


            //        //Now we need to get the full details for each of the financingstatement
            //        foreach (var financialStatement in financialStatements)
            //        {

            //            FS.Add(financialStatement.ConvertToFSReportView());
            //            Lenders.AddRange(financialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
            //            Borrowers.AddRange(financialStatement.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
            //            Collaterals.AddRange(financialStatement.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId == s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
            //            Activities.AddRange(financialStatement.FinancialStatementActivities.Where(s => s.isApproved == true && s.IsDeleted == false && s.IsActive == true).ConvertToFSActivitySummaryReportView());
            //            OtherIdentifications.AddRange(financialStatement.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());

            //            //Now let's get the updates
            //            foreach (FinancialStatementActivity fsActivity in financialStatement.FinancialStatementActivities.Where(s => s.isApproved == true && s.IsDeleted == false && s.IsActive == true).OrderByDescending(d => d.CreatedOn))
            //            {
            //                if (fsActivity is ActivityUpdate)
            //                {

            //                    FinancialStatement previousFS = _financialStatementRepository.FindBy(((ActivityUpdate)fsActivity).PreviousFinancialStatement.Id);
            //                    FS.Add(previousFS.ConvertToFSReportView());
            //                    Lenders.AddRange(previousFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
            //                    Borrowers.AddRange(previousFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
            //                    Collaterals.AddRange(previousFS.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId != s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
            //                    OtherIdentifications.AddRange(previousFS.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());

            //                    if (previousFS.AfterUpdateFinancialStatementId != financialStatement.Id)
            //                    {
            //                        FinancialStatement afterUpdateFS = previousFS.AfterUpdateFinancialStatement;
            //                        FS.Add(afterUpdateFS.ConvertToFSReportView());
            //                        Lenders.AddRange(afterUpdateFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsDeleted == false).ConvertToSecuredPartyReportView());
            //                        Borrowers.AddRange(afterUpdateFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsDeleted == false).ConvertToDebtorReportView());
            //                        Collaterals.AddRange(afterUpdateFS.Collaterals.Where(s => s.IsDeleted == false && (s.IsDischarged == false || (s.DischargeActivityId != s.FinancialStatement.DischargeActivityId))).ConvertToCollateralReportViews());
            //                        OtherIdentifications.AddRange(afterUpdateFS.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView());

            //                    }
            //                    //Add the updates details
            //                    UpdateActivityReportView updatereport = (UpdateActivityReportView)fsActivity.ConvertToActivityReportView();
            //                    updatereport.BeforeUpdateFinancialStatementId = previousFS.Id;
            //                    updatereport.FinancialStatementId = (int)previousFS.AfterUpdateFinancialStatementId;
            //                    Updates.Add(updatereport);

            //                    string xmlchangeDescription = ((ActivityUpdate)fsActivity).UpdateXMLDescription;
            //                    IEnumerable<ChangeDescriptionView> _ChangeDescription = new List<Views.FinancialStatement.ChangeDescriptionView>();
            //                    _ChangeDescription = ((ActivityUpdate)fsActivity).GetOperationDescription(xmlchangeDescription).ConvertToChangeDescriptionView();
            //                    foreach (var item in _ChangeDescription)
            //                    {
            //                        item.UpdateId = fsActivity.Id;
            //                    }
            //                    ChangeDescription.AddRange(_ChangeDescription);
            //                    //**Add the change description details
            //                }


            //                else if (fsActivity is ActivitySubordination)
            //                {

            //                    SubordinationActivity.Add((SubordinationActivityReportView)fsActivity.ConvertToActivityReportView());
            //                    //Now create the view for the activity summary and the discharged collaterals

            //                    SubordinatingParty.Add(((ActivitySubordination)fsActivity).SubordinatingParticipant.ConvertToSubordinatingPartyReportView());
            //                }
            //                else if (fsActivity is ActivityDischarge)
            //                {

            //                    DischargeActivity.Add((DischargeActivityReportView)fsActivity.ConvertToActivityReportView());
            //                    //Now create the view for the activity summary and the discharged collaterals

            //                    if (((ActivityDischarge)fsActivity).DischargeType == 2)
            //                    {
            //                        DischargedCollaterals.AddRange(((ActivityDischarge)fsActivity).Collaterals.ConvertToCollateralReportViews());
            //                    }

            //                }
            //                else if (fsActivity is ActivityAssignment)
            //                {

            //                    AssignmentActivity.Add((AssignmentActivityReportView)fsActivity.ConvertToActivityReportView());
            //                    //Now create the view for the activity summary and the discharged collaterals

            //                    Model.Membership.Membership _membership = _membershipRepository.FindBy(((ActivityAssignment)fsActivity).AssignedMembershipId);
            //                    AssignedParty.Add(((ActivityAssignment)fsActivity).AssignedMembership.ConvertToClientReportView(_institutionRepository, _userRepository));

            //                }
            //            }

            //        }
            //    }





            //    //}

            //    SearchReportGenerator report = new SearchReportGenerator(SearchParam,FS, Lenders, Borrowers, Collaterals, Activities,Updates ,ChangeDescription ,OtherIdentifications,DischargeActivity,AssignmentActivity , SubordinationActivity ,AssignedParty ,SubordinatingParty ,DischargedCollaterals   );

            //    Byte[] mybytes = report.GenerateReport();


            //    FileUpload _fileUpload = null;
            //    //Does it have a fileupload
            //    if (request.isCertified)
            //    {
            //        response.IsCertified = true;
            //        AuditAction = Model.FS.Enums.AuditAction.GenerateCertifiedSearchResult;

            //        if (searchFS.SearchRequestCertifiedResultId != null)
            //        {
            //            response.ReportAlreadyExisted = true;
            //            _fileUpload = searchFS.SearchRequestCertifiedResult;
            //            auditTracker.Updated.Add(_fileUpload);
            //        }
            //        else
            //        {

            //            ConfigurationFee configurationFee = _configurationFeeRepository.GetDbSetComplete().Where(s => s.ServiceFeeType.Any(t => t.Id == ServiceFees.CertifiedSearchResult) && s.IsActive == true && s.IsDeleted == false).FirstOrDefault();
            //            if (configurationFee != null)
            //            {
            //                PaymentServiceModel ps = new PaymentServiceModel(_accountTransactionRepository, _configurationFeeRepository, auditTracker, AuditedDate);
            //                try
            //                {
            //                    if (!request.IsPublicUser)
            //                    {

            //                        if (ps.ProcessRegisteredUserSearch(configurationFee, _membershipRepository.FindBy(request.SecurityUser.MembershipId), searchFS, true))
            //                            response.PaymentDeducted = true;//We really need to check to be sure
            //                    }
            //                    else
            //                    {
            //                        ps.ProcessUnRegisteredUserSearch(configurationFee, publicUser, searchFS, true);
            //                    }

            //                }
            //                catch (NotEnoughBalance ex)
            //                {

            //                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
            //                    response.MessageInfo.Message = ex.Message;
            //                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
            //                    return response;
            //                }
            //            }
            //        }                


            //        AuditAction = Model.FS.Enums.AuditAction.GenerateCertifiedSearchResult  ;


            //    }
            //    else
            //    {
            //        AuditAction = Model.FS.Enums.AuditAction.GenerateUnCertifiedSearchResult ;

            //         if (searchFS.SearchRequestResultId  != null)
            //        {

            //            _fileUpload = searchFS.SearchRequestResult ;
            //            auditTracker.Updated.Add(_fileUpload);
            //        }

            //    }
            //    if (_fileUpload == null)
            //    {
            //        _fileUpload = new FileUpload();
            //        auditTracker.Created.Add(_fileUpload);
            //         _fileUpload.AttachedFile = mybytes;
            //    _fileUpload.AttachedFileName = searchFS.SearchCode + ".pdf";
            //    _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            //    _fileUpload.AttachedFileType = "PDF";
            //        //**We need to update this so that we keep only one report since it can be gotten through a parameter
            //    if (!request.isCertified)
            //    {
            //        searchFS.SearchRequestResult = _fileUpload;
            //        searchFS.GeneratedUnCertifiedSearchReport = true;
            //    }
            //    else
            //    {
            //        searchFS.SearchRequestCertifiedResult  = _fileUpload;
            //        searchFS.GeneratedCertifiedSearchReport  = true;
            //    }
            //    auditTracker.Updated.Add(searchFS);
            //    }




            //    if (request.SendAsMail)
            //    {
            //        //Generate email
            //        List<Email> Emails = new List<Email>();
            //        Email email = new Email();
            //        if (request.IsPublicUser)
            //        {
            //            email.EmailTo = request.publicUsrEmail.Replace (',',';');
            //        }
            //        else
            //        {
            //            email.EmailTo = request.SecurityUser.Email;
            //        }
            //        //email.EmailTo = request.SecurityUser.Email;
            //        email.IsSent = false;
            //        email.NumRetries = 0;


            //        //Generate template
            //        EmailTemplate emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "SearchResponseMail").SingleOrDefault();
            //        EmailTemplateGenerator.SearchResponse (email, emailTemplate, _fileUpload);
            //        _emailRepository.Add(email);
            //        auditTracker.Created.Add(email);
            //        response.EmailSent = true;
            //    }
            //    int SecurityUserId;
            //    if (request.SecurityUser == null)
            //    {
            //        SecurityUserId = 1;
            //    }
            //    else
            //    {
            //        SecurityUserId = request.SecurityUser.Id  ;
            //    }
            //    //Generate the fileupload
            //    string AuditMessage = "Search Code: "+ searchFS .SearchCode;
            //    Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP,AuditAction );
            //    _auditRepository.Add(audit);
            //    auditTracker.Created.Add(audit);
            //    _uow.AuditEntities(SecurityUserId, AuditedDate, auditTracker);

            //    _uow.Commit();

            //    //Now we generate response and response must have the file


            //    //Generate the report

            //    //**We must d one query wit


            //    //We need top generate the report

            //    //We need to return the response with the report and store the report in the search page



        }

        public ViewSearchesResponse ViewSearchFS(ViewSearchesFSRequest request)
        {
            var response = _searchFinancialStatementRepository.SelectSearchesGridViewCQ(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public DownloadSearchReportResponse DownloadSearchReport(DownloadSearchReportRequest request)
        {
            DownloadSearchReportResponse response = new DownloadSearchReportResponse();
            PublicUserSecurityCode publicUser = null;

            var searchFs = _searchFinancialStatementRepository.FindBy(request.Id);
            //Get the audited date
            var searchResultTracker = _trackSearchResultRepository.GetSearchResultTrackers(request.Id).SingleOrDefault(s => s.RegistrationNo == request.RegistrationNo);

            if (request.SecurityUser != null && request.SecurityUser.IsOwnerUser == false)
            {
                if (searchFs.CreatedByUser.InstitutionId != request.SecurityUser.InstitutionId)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                    return response;
                }

                //check the 
            }
            else
            {
                publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
                if (publicUser == null)
                {
                    response.MessageInfo.MessageType = MessageType.Error;
                    response.MessageInfo.Message = "The public security user code is invalid or expired!";
                }


            }

            FileUpload _fileUpload = null;



            if ((searchResultTracker == null && request.RegistrationNo == "") || ((searchResultTracker == null || searchResultTracker.FileUploadId == null) && searchFs.GeneratedReportId != null))
            {
                _fileUpload = searchFs.GeneratedReport;
                AuditAction = Model.FS.Enums.AuditAction.DownloadExistingCertifiedSearchResult;
            }
            else if (searchResultTracker.FileUploadId != null)
            {
                _fileUpload = searchResultTracker.FileUpload;
                AuditAction = Model.FS.Enums.AuditAction.DownloadExistingCertifiedSearchResult;

            }
            else
            {
                throw new Exception("Report was supposed to be found unless it has been deleted for some strange reason");//for now throw an error
            }



            //Generate the fileupload
            string AuditMessage = "Search Code: " + searchFs.SearchCode;
            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);

            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);


            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            _uow.Commit();

            response.AttachedFile = _fileUpload.AttachedFile;
            response.AttachedFileName = _fileUpload.AttachedFileName;
            response.AttachedFileSize = _fileUpload.AttachedFileSize;
            response.AttachedFileType = _fileUpload.AttachedFileType;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public ValidateSecurityCodeResponse ValidateSecurityCode(ValidateSecurityCodeRequest request)
        {
            ValidateSecurityCodeResponse response = new ValidateSecurityCodeResponse();
            PublicUserSecurityCode publicUserSecurityCode = _publicUserSecurityCodeRepository.GetDbSet().SingleOrDefault(s => s.SecurityCode == request.SecurityCode && s.IsActive == true);
            if (publicUserSecurityCode != null)
            {

                Payment payment = _paymentRepository.GetPaymentBySecurityCode(publicUserSecurityCode.Id);

                if (payment.PaymentSource == PaymentSource.InterswitchDirectPay)
                {
                    var interSwitchDirectPay = payment.InterSwitchDirectPayTransaction;
                    if (!String.IsNullOrWhiteSpace(interSwitchDirectPay.CustName) && !String.IsNullOrWhiteSpace(interSwitchDirectPay.CustPhone))
                    {
                        response.HasInfo = true;
                    }
                }
                else if (payment.PaymentSource == PaymentSource.InterSwitchWebPay)
                {
                    var interSwitchWebPay = payment.InterSwitchWebPayTransaction;
                    if (!String.IsNullOrWhiteSpace(interSwitchWebPay.CustName) && !String.IsNullOrWhiteSpace(interSwitchWebPay.CustPhone))
                    {
                        response.HasInfo = true;
                    }
                }

                response.PaymentType = payment.PaymentSource;
                response.IsValid = true;
                response.Balance = publicUserSecurityCode.Balance;
                return response;
            }

            return response;
        }

        public AssignFSToSearchResponse AssignFSToSearch(AssignFSToSearchRequest request)
        {
            AssignFSToSearchResponse response = new AssignFSToSearchResponse();
            ServiceRequest serRequest = null;
            PublicUserSecurityCode publicUser = null;
            #region Validation
            //For institution user who is not owner allow only the search role to search
            if (Constants.ApplySearchRoleToSearch && request.SecurityUser != null && request.SecurityUser.InstitutionId != null)
            {
                if (!request.SecurityUser.IsInRole("Search"))
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }

            }
            #endregion

            #region Load Necessary Data
            //Loads an associated service request if we passed a unique identifier.  This will be validated to check if this
            //request has not already been handled
            if (String.IsNullOrWhiteSpace(request.UniqueGuidForm) == false)
                serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();
            if (request.SecurityUser == null && request.SecurityCodeForPublicUser != null)
            {
                publicUser = _publicUserSecurityCodeRepository.GetDbSet().Where(s => s.SecurityCode == request.SecurityCodeForPublicUser && s.IsActive == true).SingleOrDefault();
                if (publicUser == null)
                {
                    response.MessageInfo.MessageType = MessageType.Error;
                    response.MessageInfo.Message = "The PIN Code is invalid or expired!";
                    return response;
                }
                request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            }
            #endregion


            var mySearch = _searchFinancialStatementRepository.FindBy(request.SearchId);



            if (mySearch.SelectedFSId == null)
            {
                mySearch.SelectedFSId = request.SelectedFS;

                auditTracker.Updated.Add(mySearch);
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

                try
                {
                    _uow.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (mySearch.SelectedFSId != null && mySearch.SelectedFSId != request.SelectedFS)
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Unauthorized;
                response.MessageInfo.Message = "Your search has already been assigned to a different Financing Statement";
                return response;
            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public TrackSearchResultResponse TrackSearchResult(TrackSearchResultRequest request)
        {
            TrackSearchResultResponse response = new TrackSearchResultResponse();

            //if (request.SecurityUser == null && request.SecurityCodeForPublicUser == null && request.PayableTransaction)
            //{
            //    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Unauthorized;
            //    response.MessageInfo.Message = "Unauthorized Access";
            //    return response;
            //}

            if (request.SecurityUser == null && (request.SecurityCodeForPublicUser != null || !request.PayableTransaction))
            {
                request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            }

            SearchResultTracker searchResultTracker
                = _trackSearchResultRepository.GetDbSet().SingleOrDefault(s => s.RegistrationNo == request.RegistrationNo && s.SearchFinancialStatementId == request.SearchFinancialStatementId && s.IsActive == true && s.IsDeleted == false);
            if (!request.IsGenerateReport)
            {
                if (Constants.EXPIRE_RESULTINSEARCH)
                {
                    if (Constants.MAX_NUM_SEARCHRESULTS > 0)
                    {
                        var allSearchResultsViewCount = _trackSearchResultRepository.GetDbSet().Where(s => s.SearchFinancialStatementId == request.SearchFinancialStatementId && s.IsActive == true && s.IsDeleted == false).Select(s => (int?)s.SearchResultViewCount).Sum();
                        if (allSearchResultsViewCount >= Constants.MAX_NUM_SEARCHRESULTS)
                        {
                            response.MessageInfo.MessageType = MessageType.Error;
                            response.MessageInfo.Message = "Sorry! you have reached the limit for viewing this search results.";
                            return response;
                        }
                    }

                    if (searchResultTracker != null && searchResultTracker.SearchResultViewCount >= Constants.RESULTSEARCH_LIMIT)
                    {
                        response.MessageInfo.MessageType = MessageType.Error;
                        response.MessageInfo.Message = "Sorry! this search result has expired.";
                        return response;
                    }

                }

                if (searchResultTracker == null)
                {
                    searchResultTracker = new SearchResultTracker()
                    {
                        RegistrationNo = request.RegistrationNo,
                        SearchFinancialStatementId = request.SearchFinancialStatementId,
                        SearchResultViewCount = 1,
                        ReportGenerationCount = 0
                    };
                    _trackSearchResultRepository.Add(searchResultTracker);
                    auditTracker.Created.Add(searchResultTracker);
                }
                else
                {
                    searchResultTracker.SearchResultViewCount = searchResultTracker.SearchResultViewCount + 1;
                    auditTracker.Updated.Add(searchResultTracker);
                }
            }
            else
            {
                if (Constants.LIMIT_SEARCH_REPORT && searchResultTracker != null)
                {
                    if (searchResultTracker.ReportGenerationCount >= Constants.MAX_NUM_SEARCHREPORTS)
                    {
                        response.MessageInfo.MessageType = MessageType.Error;
                        response.MessageInfo.Message = "Sorry! you have reached search report generation limit for this result.";
                        return response;
                    }
                }
                if (searchResultTracker != null)
                {
                    var _searchFs = _searchFinancialStatementRepository.FindBy(request.SearchFinancialStatementId);
                    searchResultTracker.ReportGenerationCount = searchResultTracker.ReportGenerationCount + 1;
                    if (searchResultTracker.FileUploadId == null)
                    {
                        searchResultTracker.FileUploadId = _searchFs.GeneratedReportId;
                        searchResultTracker.ReportGenerationDate = DateTime.Now;
                    }

                    auditTracker.Updated.Add(searchResultTracker);
                }
            }

            if (searchResultTracker != null && searchResultTracker.FileUploadId != null)
            {
                response.HasSearchReport = true;
            }

            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            try
            {
                _uow.Commit();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return response;
        }

        public GetExpiredSearchResultResponse GetExpiredSearchResults(GetExpiredSearchResultRequest request)
        {
            GetExpiredSearchResultResponse response = new GetExpiredSearchResultResponse();

            if (request.SecurityUser == null && request.SecurityCodeForPublicUser == null)
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Unauthorized;
                response.MessageInfo.Message = "Unauthorized Access";
                return response;
            }

            var searchResultTracker = _trackSearchResultRepository.GetSearchResultTrackers(request.SearchFinancialStatementId);
            response.ExpiredResultsRegNo = new List<string>();
            response.GeneratedSearchReportsRegNo = new List<string>();
            ICollection<SearchReportView> searchReports = new List<SearchReportView>();
            foreach (var searchResult in searchResultTracker)
            {
                if (Constants.EXPIRE_RESULTINSEARCH)
                {
                    if (searchResult != null && searchResult.SearchResultViewCount >= Constants.RESULTSEARCH_LIMIT)
                    {
                        response.ExpiredResultsRegNo.Add(searchResult.RegistrationNo);
                    }
                }

                if (searchResult != null && searchResult.FileUploadId != null)
                {
                    response.GeneratedSearchReportsRegNo.Add(searchResult.RegistrationNo);

                    if (Constants.SHOW_GENERATED_REPORTS_LIST)
                    {
                        searchReports.Add(new SearchReportView()
                        {
                            RegistrationNo = searchResult.RegistrationNo,
                            SearchId = searchResult.SearchFinancialStatementId,
                            DateGenerated = (DateTime)searchResult.ReportGenerationDate,
                            AttachedFileSize = searchResult.FileUpload.AttachedFileSize,
                            AttachedFileName = searchResult.FileUpload.AttachedFileName,
                            AttachedFileType = searchResult.FileUpload.AttachedFileType
                        });
                    }
                }
            }
            if (Constants.SHOW_GENERATED_REPORTS_LIST)
            {
                response.SearchReports = searchReports;
                if (response.SearchReports.Count() > 0)
                {
                    response.HasSearchReports = true;
                }
            }
            //if (searchResultTracker.Count() == response.ExpiredResultsRegNo.Count())
            //{
            //    response.HasExpiredAllResults = true;
            //}

            if (Constants.EXPIRE_RESULTINSEARCH)
            {
                if (Constants.MAX_NUM_SEARCHRESULTS > 0)
                {
                    var allSearchResultsViewCount = _trackSearchResultRepository.GetDbSet().Where(s => s.SearchFinancialStatementId == request.SearchFinancialStatementId && s.IsActive == true && s.IsDeleted == false).Select(s => (int?)s.SearchResultViewCount).Sum();
                    if (allSearchResultsViewCount >= Constants.MAX_NUM_SEARCHRESULTS)
                    {
                        response.HasExpiredAllResults = true;
                        return response;
                    }
                }
            }


            return response;
        }

        public ResponseBase AuditCacLink(RequestBase request)
        {
            ResponseBase response = new ResponseBase();
            AuditAction = Model.FS.Enums.AuditAction.DownloadExistingCertifiedSearchResult;
            string AuditMessage = "A user searched on CAC";
            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);

            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            _uow.Commit();
            response.MessageInfo.MessageType = MessageType.Success;
            return response;
        }

        public CACSearchresponse GetCACSearchResults(CACSearchrequest request)
        {
            CACSearchresponse response = new CACSearchresponse();
            //var searchResults = new CACSearch();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(request.apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var CACresponse = client.GetAsync(request.apiUrl).Result;
                if (CACresponse.IsSuccessStatusCode)
                {
                    response.CACResults = CACresponse.Content.ReadAsStringAsync().Result;
                }
            }

            //var search = _searchFinancialStatementRepository.GetDbSet().Where(s=>s.UniqueIdentifier == request.uniqueId).FirstOrDefault();
            //if (search != null)
            //{
            //    search.CACResultsXML = SearchParameterHelper.GenerateXML<string>(request.CACResults);

            //    auditTracker.Updated.Add(search);
            //    var audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, Model.FS.Enums.AuditAction.SearchUsingCACLink);
            //    _auditRepository.Add(audit);
            //    auditTracker.Created.Add(audit);
            //    if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            //    _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            //    try
            //    {
            //        _uow.Commit();
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}

            return response;
        }
    }
}
