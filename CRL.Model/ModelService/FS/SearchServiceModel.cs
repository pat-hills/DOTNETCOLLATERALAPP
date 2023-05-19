using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.Notification.IRepository;
using CRL.Model.Search;
using CRL.Model.Search.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Messaging;
using CRL.Model.Model.Search;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.ModelService.FS
{
    public class SearchServiceModel : FSServiceModelBase
    {


        private ISearchFinancialStatementRepository _searchFinancialStatementRepository;
        private IParticipantRepository _participantRepository;
        private ICollateralRepository _collateralRepository;
        private IInstitutionRepository _institutionRepository;
        private IUserRepository _userRepository;
        public SearchFinancialStatement fsSch = null;

        public bool ValidateSecurity()
        {
            return true;
        }

        public SearchServiceModel(ISerialTrackerRepository serialTrackerRepository,
            IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, ISearchFinancialStatementRepository searchFinancialStatementRepository,
            IFinancialStatementRepository financialStatementRepository, IParticipantRepository participantRepository, ICollateralRepository collateralRepository, IFinancialStatementActivityRepository fsActivityRepository,
            IInstitutionRepository institutionRepository, IUserRepository userRepository, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _searchFinancialStatementRepository = searchFinancialStatementRepository;
            _serialTrackerRepository = serialTrackerRepository;
            _financialStatementRepository = financialStatementRepository;
            _tracker = tracker;
            _executingUser = user;
            _serviceRequest = ServiceRequest;
            _collateralRepository = collateralRepository;
            _participantRepository = participantRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _institutionRepository = institutionRepository;
            _userRepository = userRepository;


        }

        public void LoadInitialDataFromRepository(int SearchRequestId)
        {

            if (SearchRequestId > 0)
            {
                fsSch = _searchFinancialStatementRepository.FindBy(SearchRequestId);
            }
        }

        public void GenerateResult(GenerateSearchReportRequest request, GenerateSearchReportResponse response)
        {
            response.SearchId = fsSch.Id;


        }

        public SearchFinancialStatement Search(SearchRequest request, SearchResponse response)
        {
            string[] FoundRegistrationNos;
            //Find the registration nos
            FoundRegistrationNos = _searchFinancialStatementRepository.Search(request);

            //Get all financing statements with registration nos
            IQueryable<FinancialStatement> query = _financialStatementRepository.GetDbSetComplete().Where(s => s.IsActive == true
                && FoundRegistrationNos.Contains(s.RegistrationNo) && s.AfterUpdateFinancialStatementId == null);
            int TotalRecords = query.Count();
            response.NumRecords = TotalRecords;
            response.RegistrationNos = FoundRegistrationNos;

            //Why are we rerunning this query again, maybe because after the count we cannot requery again with the query
            var reqtrieveFSQuery = _financialStatementRepository.GetDbSetComplete().Where(s => s.IsActive == true
                && FoundRegistrationNos.Contains(s.RegistrationNo) && s.AfterUpdateFinancialStatementId == null); ;
            reqtrieveFSQuery = reqtrieveFSQuery.OrderBy(s => s.Id);
            if (request.PageIndex > 0)
                reqtrieveFSQuery = reqtrieveFSQuery.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            //Create the search view, this is heavy with lots of sql access
            response.SearchResultView = _financialStatementRepository.FindBy(reqtrieveFSQuery).ToList().ConvertToSearchResultView(request, _participantRepository, _collateralRepository);

            //Add an encrypted id to the search result, may not be needed anymore
            foreach (var result in response.SearchResultView)
            {
                result.EncryptedId = Util.Encrypt(result.Id.ToString());
            }

            //Fill the response class
            response.FilterSearch = new FilterSearch();
            response.FilterSearch.RegistrationNo = FoundRegistrationNos;
            response.NumRecords = TotalRecords;
            response.RegistrationNos = FoundRegistrationNos;


            // Create the new financing statement
            //Now we need to map the request class to the ser..
            fsSch = new SearchFinancialStatement();
            _searchFinancialStatementRepository.Add(fsSch);
            _tracker.Created.Add(fsSch);
            //Generate a string for the search parameters
            fsSch.SearchParamXML = SearchParameterHelper.GenerateXML<SearchParam>(request.SearchParam);
            fsSch.SearchParamString = request.SearchParam.ParameterString;
            fsSch.UniqueIdentifier = request.UniqueIdentifier;
            fsSch.FoundRegistrationXML = SearchParameterHelper.GenerateXML<string[]>(FoundRegistrationNos);
            fsSch.FoundRegistrationString = String.Join(",", FoundRegistrationNos);
            fsSch.NumRecords = response.NumRecords;
            if (request.IsNonLegalEffectSearch)
            {
                fsSch.IsLegalOrFlexible = 2;

            }
            else
            {

                fsSch.IsLegalOrFlexible = 1;
            }
            //fsSch.se = "Not implemented yet"; 

            if (request.SecurityUser == null)
            {

                fsSch.IsPublicUser = true;
                fsSch.PublicUsrCode = request.SecurityCodeForPublicUser;



            }
            else
            {
                fsSch.InstitutionUnitId = _executingUser.InstitutionUnitId;
            }

            return fsSch;

        }

        public void AssignSearchNo()
        {
            fsSch.SearchCode = "SCH" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Search); ;

        }

        public void GenerateSearchReport(SearchFSReportBuilder fsActivityVerificationBuilder, List<FinancialStatement> SelectedFS, LookUpForFS lookUpForFS)
        {
            List<ClientReportView> AssigneeNAssignor = null;
            foreach (var fs in SelectedFS)
            {
                var activities = fs.FinancialStatementActivities.Where(s => s.isApprovedOrDenied == 1 && s.IsDeleted == false && s.IsActive == true && s.FinancialStatementActivityTypeId == FinancialStatementActivityCategory.FullAssignment);
                {
                    foreach (var activity in activities)
                    {
                        AssigneeNAssignor = new List<ClientReportView>();
                        var AssignedClientReportView = (((ActivityAssignment)activity).AssignedMembership.ConvertToClientReportView(_institutionRepository, _userRepository));
                        var AssignedFromClientReportView = (((ActivityAssignment)activity).AssignedMembershipFrom.ConvertToClientReportView(_institutionRepository, _userRepository));
                        if (AssigneeNAssignor.Where(s => s.Id == AssignedClientReportView.Id).Count() < 1)
                        {
                            AssigneeNAssignor.Add(AssignedClientReportView);
                        }
                        if (AssigneeNAssignor.Where(s => s.Id == AssignedFromClientReportView.Id).Count() < 1)
                        {
                            AssigneeNAssignor.Add(AssignedFromClientReportView);
                        }


                    }
                }
            }

            //Generate the veridication report         
            FileUpload _fileUpload = GenerateSearchFSReport(fsSch, SelectedFS, fsActivityVerificationBuilder, AssigneeNAssignor, lookUpForFS, _tracker);

            

            //Email Notification = new Email();
            //Notification.EmailTo = _executingUser.Email;  //Workflowemails not added but will be part of this list
            //EmailUserAssignment emailAssignment = new EmailUserAssignment();
            //emailAssignment.UserId = _executingUser.Id;
            //emailAssignment.IsActive = true;
            //emailAssignment.IsRead = false;
            //emailAssignment.Email = Notification;
            //_tracker.Created.Add(emailAssignment);
            //_emailUserAssignmentRepository.Add(emailAssignment);
            //_emailRepository.Add(Notification);
            //_tracker.Created.Add(Notification);
            //EmailTemplateGenerator.CreateFinancingStatementMail(Notification, submittedFS, FSActivityEmailTemplate);
            // EmailTemplateGenerator.CreateAmendmentMail(Notification, fsActivity, FSActivityEmailTemplate, _fileUpload);

        }

    }
}
