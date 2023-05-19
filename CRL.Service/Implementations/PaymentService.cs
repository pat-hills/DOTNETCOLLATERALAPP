using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Notification.IRepository;


using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Mappings.Payments;
using CRL.Infrastructure.Domain;

using CRL.Service.BusinessServices;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Memberships.Request;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Helpers;
using CRL.Model.Notification;
using CRL.Service.QueryGenerator;
using CRL.Service.Views.Payments;
using Microsoft.Reporting.WebForms;
using CRL.Infrastructure.Configuration;
using CRL.Service.Messaging;
using CRL.Model.FS.Enums;

using CRL.Service.BusinessService;
using System.Data.Entity.Infrastructure;
using CRL.Repository.EF.All.Repository.Payments;
using CRL.Model.ModelViews;
using CRL.Model.ModelService;
using CRL.Model.ModelViews.Payments;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Enums;
using CRL.Model.Messaging;
using CRL.Service.Common;

using CRL.Service.Messaging.FinancialStatements.Request;
using MicrosoftReportGenerators;
using CRL.Model.FS;
using CRL.Model.Payments;
using CRL.Model.Memberships;
using CRL.Model.ModelViews.Memberships;
using CRL.Service.Messaging.Common.Request;
using System.IO;
using Newtonsoft.Json;

namespace CRL.Service.Implementations
{
    public class PaymentService : ServiceBase, IPaymentService
    {

        //private readonly IInstitutionRepository _institutionRepository;
        //private readonly IUserRepository _userRepository;
        //private readonly IEmailTemplateRepository _emailTemplateRepository;
        //private readonly IEmailRepository _emailRepository;
        //private readonly ISerialTrackerRepository _serialTrackerRepository;
        //private readonly IPaymentRepository _paymentRepository;
        //private readonly IAccountTransactionRepository _accountTransactionRepository;
        //private readonly IAccountBatchRepository _accountBatchRespository;
        //private readonly IMembershipRepository _membershipRepository;
        //private readonly IAuditRepository _auditRepository;
        //private readonly IServiceRequestRepository _serviceRequestRepository;
        //private readonly IInterSwitchRepository _interSwitchRepository;
        //private readonly IInterSwitchDirectPayRepository _interSwitchDirectPayRepository;
        //private readonly IUnitOfWork _uow;
        //public AuditingTracker auditTracker { get; set; }
        //public DateTime AuditedDate { get; set; }
        //public String AuditMessage;
        //public AuditAction AuditAction { get; set; }

        //public PaymentService(
        //    IInstitutionRepository institutionRepository,
        // IUserRepository userRepository,
        // IEmailTemplateRepository emailTemplateRepository,
        //      ISerialTrackerRepository serialTrackerRepository,
        // IEmailRepository emailRepository,
        //    IAccountTransactionRepository accountTransactionRepository,
        //    IAccountBatchRepository accountBatchRespository,
        //    IPaymentRepository paymentRepository,
        //    IMembershipRepository membershipRepository,
        //    IAuditRepository auditRepository,
        //        IServiceRequestRepository serviceRequestRepository,
        //    IInterSwitchRepository interSwitchRepository,
        //    IInterSwitchDirectPayRepository interSwitchDirectPayRepository,
        //                    IUnitOfWork uow)
        //{
        //    AuditedDate = DateTime.Now;
        //    auditTracker = new AuditingTracker();
        //    _institutionRepository = institutionRepository;
        //    _userRepository = userRepository;
        //    _emailTemplateRepository = emailTemplateRepository;
        //    _emailRepository = emailRepository;
        //    _serialTrackerRepository = serialTrackerRepository;
        //    _paymentRepository = paymentRepository;
        //    _accountTransactionRepository = accountTransactionRepository;
        //    _accountBatchRespository = accountBatchRespository;
        //    _membershipRepository = membershipRepository;
        //    _auditRepository = auditRepository;
        //    _serviceRequestRepository = serviceRequestRepository;
        //    _interSwitchRepository = interSwitchRepository;
        //    _interSwitchDirectPayRepository = interSwitchDirectPayRepository;
        //    _uow = uow;
        //}

        public GetDataForSelectClientResponse GetDataForSelectClient(int? membershipId = null)
        {
            GetDataForSelectClientResponse response = new GetDataForSelectClientResponse();
            response.IndividualClients = LookUpServiceModel.IndividualClients(_userRepository); //**caching may be necessary                
            response.LegalEntityClients = LookUpServiceModel.LegalEntityClients(_institutionRepository, membershipId); //**caching maybe necessary
            response.MessageInfo = new MessageInfo { MessageType = MessageType.Success };
            return response;
        }
        public GetDataForViewPaymentResponse GetView(GetDataForViewPaymentRequest request)
        {
            PaymentView paymentView = _paymentRepository.GetPaymentView(request.Id);
            GetDataForViewPaymentResponse response = new GetDataForViewPaymentResponse();
            response.PaymentView = paymentView;
            return response;



        }

        public MakePaymentResponse MakeReversalPayment(RequestBase request)
        {
            MakePaymentResponse response = new MakePaymentResponse();

            ServiceRequest serRequest = null;

            if (!String.IsNullOrWhiteSpace(request.UniqueGuidForm))
            {
                serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();
                if (serRequest != null)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "This request has already been submitted successfully.  Click on the Create New Financing Statement from the Financing Statement nenu" };
                    return response;
                }
                else
                {
                    serRequest = new ServiceRequest();
                    serRequest.RequestNo = request.UniqueGuidForm;
                    _serviceRequestRepository.Add(serRequest);


                }
            }
            PaymentAccountTransaction accountTransaction = new PaymentAccountTransaction();
            Payment payment = _paymentRepository.FindBy(request.Id);
            Membership membership = null;
            PublicUserSecurityCode publicUser = null;
            string EmailAddress = null;

            if (payment != null)
            {
                if (payment.MembershipId != null)
                    membership = payment.Membership;
                else
                    publicUser = payment.PublicUserSecurityCode;
            }
            if (payment != null)
            {
                if (payment.PaymentType == PaymentType.Reversal)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "Cannot reverse an already reversed payment!" };

                    return response;
                }
                Payment Reversalpayment = new Payment()
                {
                    Amount = payment.Amount,
                    PaymentType = PaymentType.Reversal,
                    IsActive = true,
                    Membership = membership,
                    Payee = payment.Payee.TrimNull(),
                    PaymentNo = "PAY" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Payment),
                    T24TransactionNo = payment.T24TransactionNo.TrimNull(),
                    PaymentSource = payment.PaymentSource,
                    PaymentDate = AuditedDate,
                    PublicUserSecurityCode = publicUser


                };
                payment.ReversedPayment = Reversalpayment;
                auditTracker.Created.Add(Reversalpayment);
                auditTracker.Updated.Add(payment);
                auditTracker.Updated.Add(accountTransaction);
                _paymentRepository.Add(Reversalpayment);

                decimal amountToAdd = Reversalpayment.PaymentType == PaymentType.Reversal ? -payment.Amount : payment.Amount;
                accountTransaction.NewPrepaidBalanceAfterTransaction += amountToAdd;
                if (payment.MembershipId != null)
                    accountTransaction.NewPostpaidBalanceAfterTransaction = membership.PostpaidCreditBalance;
                accountTransaction.Payment = Reversalpayment;

                accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
                accountTransaction.CreditOrDebit = CreditOrDebit.Debit;
                accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.ReversalPayment;

                if (membership != null)
                {

                    accountTransaction.Membership = membership;
                    membership.PrepaidCreditBalance += amountToAdd;

                    auditTracker.Updated.Add(membership);
                    AuditAction = Model.FS.Enums.AuditAction.ReversedClientPayment;

                    //Check if this is an institution then get the admin password or configured password

                    if (membership.isIndividualOrLegalEntity == 1)
                    {
                        User user = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).SingleOrDefault();
                        if (user != null)
                        {
                            EmailAddress = user.Address.Email;

                        }
                        else
                        {
                            throw new Exception("No associated client with membership id" + membership.Id.ToString());
                        }
                    }
                    else
                    {
                        //**Must be made to the finance officer of the company and admin
                        List<User> users = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id && s.Roles.Any(b => b.Id == Roles.AdminClient || b.Id == Roles.FinanceOfficer)).ToList();
                        if (users.Count > 0)
                        {
                            foreach (var user in users)
                            {
                                EmailAddress += user.Address.Email + ";";
                            }

                        }

                    }
                }
                else
                {
                    publicUser.Balance = publicUser.Balance + amountToAdd;
                    auditTracker.Updated.Add(publicUser);
                    EmailAddress = publicUser.PublicUserEmail;
                    AuditAction = Model.FS.Enums.AuditAction.ReversedUnregisteredClientPayment;

                }

                //Eamiling

                //Generate email
                if (EmailAddress != null)
                {
                    Email email = new Email();
                    email.EmailTo = EmailAddress.TrimEnd(';');
                    email.IsSent = false;
                    email.NumRetries = 0;


                    //Generate template
                    EmailTemplate emailTemplate = null;

                    //**We need a configured email but now just use the administrator for membership user
                    //*8For individuals use the individual email
                    //**for public users use the mail provided

                    if (membership == null)
                    {
                        //**Encrupt the code
                        //request.UrlLink = request.UrlLink.Replace("SCHCODE",Util.GetUrlEncode ( EncryptedSecurityCode));
                        emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "ReversedPayment").SingleOrDefault();

                    }
                    else
                    {

                        emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "ReversedPayment").SingleOrDefault();

                    }

                    User receivingUser = payment.CreatedByUser;
                    string receivedBy = NameHelper.GetFullName(receivingUser.FirstName, receivingUser.MiddleName, receivingUser.Surname);
                    string paypoint = receivingUser.Institution.Name;

                    EmailTemplateGenerator.ReversalPaymentNotification(email, payment, receivedBy, paypoint, emailTemplate);
                    _emailRepository.Add(email);
                    auditTracker.Created.Add(email);
                }


                AuditMessage = "Original payment no: " + payment.PaymentNo + " | New payment no:" + Reversalpayment.PaymentNo;
                AuditMessage = PaymentAuditMsgGenerator.PaymentDetails(Reversalpayment);

                Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);

                accountTransaction.Narration = AuditMessage;
                _auditRepository.Add(audit);
                auditTracker.Created.Add(audit);
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

                try
                {
                    _uow.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                    return response;


                }
                catch
                {
                    //WFor exceptions that we can handle we will have a set of handled exceptions enumeration and 
                    //pass them back to the response call
                    throw;
                }



                //response. = GetClientInstitution(institution);
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
                response.MessageInfo.Message = "Payment reversal was successful";
                response.Id = payment.Id;
                return response;


                //Adjust the membership or the account transaction

            }
            else
            {
                throw new Exception("Payment was not supposed to be null");
            }

        }

        public GetClientSummaryViewResponse GetClientSummaryView(GetClientSummaryViewRequest request)
        {
            GetClientSummaryViewResponse response = new GetClientSummaryViewResponse();
            Membership membership = null;
            ClientSummaryView clientSummaryView = null;


            if (request.MembershipId == null)
            {
                membership = _membershipRepository.GetDbSet().Where(s => s.ClientCode == request.ClientCode).SingleOrDefault();

            }
            else
            {
                membership = _membershipRepository.FindBy((int)request.MembershipId);
            }
            if (membership != null)
            {
                clientSummaryView = new ClientSummaryView();
                response.ClientSummaryView = clientSummaryView;
                clientSummaryView.MembershipId = membership.Id;
                clientSummaryView.ClientCode = membership.ClientCode;

                Institution institution = _institutionRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).SingleOrDefault();
                if (institution != null)
                {
                    clientSummaryView.ClientName = institution.Name;
                    clientSummaryView.ClientPhone = institution.Address.Phone;
                    clientSummaryView.ClientEmail = institution.Address.Email;
                    clientSummaryView.ClientIsIndividualOrLegal = 2;
                    clientSummaryView.ClientType = "Institution";
                    //clientSummaryView .Balance = 
                }
                else
                {
                    User user = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id && s.InstitutionId == null).SingleOrDefault();
                    if (user != null)
                    {
                        clientSummaryView.ClientName = NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname);
                        clientSummaryView.ClientPhone = user.Address.Phone;
                        clientSummaryView.ClientEmail = user.Address.Email;
                        clientSummaryView.ClientType = "Individual";
                        clientSummaryView.ClientIsIndividualOrLegal = 1;
                    }
                    else
                    {
                        throw new Exception("Client not found but membership was found!");
                    }
                }


            }
            //Check individual

            return response;
        }
        public MakePaymentResponse MakePayment(MakePaymentRequest request)
        {

            MakePaymentResponse response = new MakePaymentResponse();
            ServiceRequest serRequest = null;

            if (!String.IsNullOrWhiteSpace(request.UniqueGuidForm))
            {
                serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();
                if (serRequest != null)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "This request has already been submitted successfully.  Click on the Create New Financing Statement from the Financing Statement nenu" };
                    return response;
                }
                else
                {
                    serRequest = new ServiceRequest();
                    serRequest.RequestNo = request.UniqueGuidForm;
                    _serviceRequestRepository.Add(serRequest);


                }
            }

            PaymentAccountTransaction accountTransaction = new PaymentAccountTransaction();
            if (!request.SecurityUser.IsPaypointUser)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }




            decimal AfterPaymentAmount = 0;
            decimal PostpaidBalance = 0;
            Payment payment = request.PaymentView.ConvertToPayment();
            payment.PaymentType = PaymentType.Normal;
            auditTracker.Created.Add(payment);
            string EncryptedSecurityCode = null;
            string EmailAddress = null;


            decimal amountToAdd = payment.PaymentType == PaymentType.Normal ? payment.Amount : -payment.Amount;

            //Check if this is a payment for a public client or annonymous client
            if (request.PaymentView.IsPublicUser)
            {
                //First of all if we chose an adjustment payment then for public user we need the security cvode
                PublicUserSecurityCode publicUser = null;

                publicUser = new PublicUserSecurityCode();
                payment.PublicUserSecurityCode = publicUser;
                //publicUser.SecurityCode = "xxx-xxxx";//Generate a new security code
                publicUser.SecurityCode = Util.GetNewValidationCode();
                publicUser.PublicUserEmail = request.PaymentView.PublicUserEmail.TrimNull();
                payment.PublicUserSecurityCode = publicUser;
                auditTracker.Created.Add(publicUser);



                EncryptedSecurityCode = publicUser.SecurityCode.TrimNull();

                publicUser.Balance = publicUser.Balance + amountToAdd;
                AfterPaymentAmount = publicUser.Balance;
                EmailAddress = publicUser.PublicUserEmail.TrimNull();



            }
            else
            {

                Membership membership = _membershipRepository.FindBy((int)payment.MembershipId);
                accountTransaction.Membership = membership;
                membership.PrepaidCreditBalance += amountToAdd;
                PostpaidBalance = membership.PostpaidCreditBalance;
                AfterPaymentAmount = membership.PrepaidCreditBalance;

                //Check if this is an institution then get the admin password or configured password
                Institution institution = _institutionRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).SingleOrDefault();
                if (institution == null)
                {
                    User user = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id && s.InstitutionId == null).SingleOrDefault();
                    if (user != null)
                    {
                        EmailAddress = user.Address.Email;

                    }
                    else
                    {
                        throw new Exception("No associated client with membership id" + membership.Id.ToString());
                    }
                }
                else
                {
                    //**Must be made to the finance officer of the company and admin
                    List<User> users = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id && s.InstitutionId == institution.Id && s.Roles.Any(b => b.Id == Roles.FinanceOfficer || b.Id == Roles.AdminClient)).ToList();
                    if (users.Count > 0)
                    {
                        foreach (var user in users)
                        {
                            EmailAddress += user.Address.Email + ";";
                        }

                    }

                }
                auditTracker.Updated.Add(membership);

            }

            //Generate payment code
            payment.PaymentNo = "PAY" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Payment);

            //Generate the accounting information

            accountTransaction.Amount = Math.Abs(payment.Amount);
            accountTransaction.CreditOrDebit = CreditOrDebit.Credit;

            //accountTransaction.CreditOrDebit = (CreditOrDebit)payment.CreditOrDebit ;          

            accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.Payment;
            accountTransaction.NewPrepaidBalanceAfterTransaction = AfterPaymentAmount;
            accountTransaction.NewPostpaidBalanceAfterTransaction = PostpaidBalance;
            accountTransaction.Payment = payment;
            accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
            auditTracker.Created.Add(accountTransaction);
            _accountTransactionRepository.Add(accountTransaction);

            //Generate email
            if (EmailAddress != null)
            {
                Email email = new Email();
                email.EmailTo = EmailAddress.TrimEnd(';');
                email.IsSent = false;
                email.NumRetries = 0;


                //Generate template
                EmailTemplate emailTemplate = null;

                //**We need a configured email but now just use the administrator for membership user
                //*8For individuals use the individual email
                //**for public users use the mail provided

                if (request.PaymentView.IsPublicUser)
                {
                    //**Encrupt the code
                    //request.UrlLink = request.UrlLink.Replace("SCHCODE",Util.GetUrlEncode ( EncryptedSecurityCode));
                    emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "UnregisteredPaymentNotification").SingleOrDefault();

                }
                else
                {

                    emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "RegisteredPaymentNotification").SingleOrDefault();

                }

                User receivingUser = _userRepository.FindBy(request.SecurityUser.Id);
                string receivedBy = NameHelper.GetFullName(receivingUser.FirstName, receivingUser.MiddleName, receivingUser.Surname);
                string paypoint = receivingUser.Institution.Name;


                EmailTemplateGenerator.PaymentNotification(email, payment, receivedBy, paypoint, emailTemplate, request.PaymentView.IsPublicUser, request.UrlLink);

                _emailRepository.Add(email);

                auditTracker.Created.Add(email);
            }

            AuditAction = request.PaymentView.IsPublicUser ? Model.FS.Enums.AuditAction.ReceivedUnRegisteredClientPayment : Model.FS.Enums.AuditAction.ReceivedRegisteredClientPayment;
            AuditMessage = PaymentAuditMsgGenerator.PaymentDetails(payment);
            accountTransaction.Narration = AuditMessage;
            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            try
            {
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update original values from the database 
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;


            }
            catch
            {
                //WFor exceptions that we can handle we will have a set of handled exceptions enumeration and 
                //pass them back to the response call
                throw;
            }



            //response. = GetClientInstitution(institution);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Payment was successful";
            response.Id = payment.Id;
            return response;

        }
        public ViewPaymentsResponse ViewPayments(ViewPaymentsRequest request)
        {

            ViewPaymentsResponse response = new ViewPaymentsResponse();
            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = PaymentQueryGenerator.CreateQueryForFindPayment(request, _paymentRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = PaymentQueryGenerator.CreateQueryForFindPayment(request, _paymentRepository, false);
            response.PaymentViews = myquery2.ToList();
            return response;
        }


        public GetReceiptResponse GetReceiptRequest(GetReceiptRequest request)
        {
            GetReceiptResponse response = new GetReceiptResponse();
            bool configAuditDownloadPaymentReceipt = true;
            PaymentView paymentView = _paymentRepository.GetPaymentView(request.Id);

            //Validate that this user can see this payment receipt
            //first if it is not bog


            bool UserIsVerified = false;

            if (request.SecurityUser == null)
            {
                request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
                UserIsVerified = true;
            }
            else if (!request.SecurityUser.IsOwnerUser)
            {
                if (request.SecurityUser.IsInAnyRoles("Administrator (Client)", "Finance Officer")) //Can check all payments made by themselves and payments they have received
                {
                    if (paymentView.MembershipId == request.SecurityUser.MembershipId || paymentView.CreatedByUserMembershipId == request.SecurityUser.MembershipId)
                    {
                        UserIsVerified = true;
                    }

                }
                else if (paymentView.CreatedBy == request.SecurityUser.Id)
                {
                    UserIsVerified = true;
                }
            }
            else
            {
                if (request.SecurityUser.IsInAnyRoles("Administrator (Owner)", "CRL Finance Officer")) //Can check all payments made by themselves and payments they have received
                {

                    UserIsVerified = true;


                }
                else if (paymentView.CreatedBy == request.SecurityUser.Id)
                {
                    UserIsVerified = true;
                }

            }

            if (!UserIsVerified)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            //Generate the fileupload
            LocalReport report = new LocalReport();
            //report.ReportPath = "\\Reporting\\FSVerificationStatement.rdlc";
            report.ReportPath = report.ReportPath = Constants.GetReportPath + "Payment\\PaymentReceipt.rdlc"; ;
            ReportDataSource rdsFS = new ReportDataSource();
            ICollection<PaymentView> paymentViews = new List<PaymentView>();
            paymentViews.Add(paymentView);
            rdsFS.Name = "dsPayment";  //This refers to the dataset name in the RDLC file
            rdsFS.Value = paymentViews;

            report.DataSources.Add(rdsFS);
            Byte[] mybytes = report.Render("PDF");


            response.AttachedFile = mybytes;
            response.AttachedFileName = "PaymentReceipt" + paymentView.PaymentNo;
            response.AttachedFileSize = mybytes.Length.ToString();
            response.AttachedFileType = "PDF"; //**Do correct pdf mime
            if (configAuditDownloadPaymentReceipt)
            {
                Audit audit = new Audit(paymentView.PaymentNo, request.RequestUrl, request.UserIP, AuditAction.DownloadPaymentReceipt);
                _auditRepository.Add(audit);
                auditTracker.Created.Add(audit);
            }
            return response;
        }


        /// <summary>
        /// Use this service to load clients for confirmation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public GetConfirmClientsInPostpaidBatchResponse GetConfirmClientsInPostpaidBatch(RequestBase request)
        {
            GetConfirmClientsInPostpaidBatchResponse response = new GetConfirmClientsInPostpaidBatchResponse();
            response.ClientsInPostpaidBatchToConfirmView = _accountTransactionRepository.GetClientForConfirmationInPostpaidBatch(request.Id);
            response.AccountBatchView = _accountBatchRepository.GetAccountBatchView(request.Id);
            return response;

        }
        public GetFileAttachmentResponse GenerateBatchDetailReport(RequestBase request)
        {

            GetFileAttachmentResponse response = new GetFileAttachmentResponse();

            MFSBatchDetailsReportBuilder verificationBuilder = new MFSBatchDetailsReportBuilder();
            AccountBatch accountBatch = _accountBatchRepository.FindBy(request.Id);
            AccountBatchView AccountBatchView = new AccountBatchView
            {
                Id = accountBatch.Id,
                ConfirmSubPostpaidAccount = accountBatch.ConfirmSubPostpaidAccount,
                InstitutionId = accountBatch.InstitutionId,
                InstitutionName = accountBatch.Institution.Name,
                Name = accountBatch.Name,
                isReconciled = accountBatch.isReconciled,
                CreatedOn = accountBatch.CreatedOn,
                PeriodStartDate = accountBatch.PeriodStartDate,
                PeriodEndDate = accountBatch.PeriodEndDate
            };

            //Load the expenditure transactions which is all transactions that are not settlement and joint to the batch
            ICollection<ExpenditureByTransactionView> ClientExpendituresInBatch = _accountTransactionRepository.ViewClientExpendituresByTransaction(request.Id);
            ICollection<ClientAmountView> PostpaidClientTotalExpenditure = _accountTransactionRepository.GetMyPostpaidClientExpenditures(request.Id);
            ICollection<ClientSettlementSummaryView> SettledPayments = _accountTransactionRepository.ViewClientSettlementSummary(request.Id);

            verificationBuilder.BuildBatchDetailReport(PostpaidClientTotalExpenditure, SettledPayments,
                ClientExpendituresInBatch, AccountBatchView);

            Byte[] mybytes = verificationBuilder.GenerateVerificationReport();
            FileUpload _fileUpload = new FileUpload();
            response.AttachedFile = mybytes;
            response.AttachedFileName = "Batch_" + ".pdf";
            response.AttachedFileSize = mybytes.Length.ToString();
            response.AttachedFileType = "application/pdf";
            return response;

        }


        public GetReconcileResponse GetBatchDetails(GetReconcileRequest request)
        {
            GetReconcileResponse response = new GetReconcileResponse();
            var myquery2 = PaymentQueryGenerator.CreateQueryForSelectBankPostpaidTransactions(request, _accountTransactionRepository, false);

            response.ClientAmountWithRepBankView = _accountTransactionRepository.ViewClientExpenditureSummary((int)request.BatchId);
            response.AccountBatchView = _accountBatchRepository.GetAccountBatchView((int)request.BatchId);



            response.SettledClientsInBatchViews = _accountTransactionRepository.ViewClientSettlementSummary((int)request.BatchId);
            return response;

        }


        public CreatePostpaidBatchResponse CreatePostpaidBatch(CreatePostpaidBatchRequest request)
        {
            //Only Client Administrators and Finance Oficers
            CreatePostpaidBatchResponse response = new CreatePostpaidBatchResponse();

            //Validate that only administrators can create new users
            if (!(request.SecurityUser.IsFinanceOfficer()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            response.BatchNo = _paymentRepository.CreateBatch(request.StartDate, request.EndDate, request.BatchType, request.Batchcomment, request.SecurityUser.Id, request.UserIP, request.RequestUrl);
            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "You successfully generated batch of postpaid transactions!", MessageType = Infrastructure.Messaging.MessageType.Success };
            return response;

        }

        public CreatePostpaidBatchResponse ReconcileBatchedPostpaidTransactions(ReconcileBatchedPostpaidTransactionsRequest request)
        {
            //Only BOG and Finance Oficers
            CreatePostpaidBatchResponse response = new CreatePostpaidBatchResponse();
            if (request.SecurityUser.IsOwnerUser)
            {
                response.BatchNo = _paymentRepository.OwnerReconcilePayment(request.BatchId, request.ReconcileComment, request.SecurityUser.Id, request.UserIP, request.RequestUrl);
            }
            else
            {
                response.BatchNo = _paymentRepository.ConfirmSubPostpaidClients(request.BatchId, request.RepresentativeInstitution, request.ReconcileComment, request.SecurityUser.Id, request.UserIP, request.RequestUrl);
            }

            //var accountBatch = _accountBatchRepository.FindBy(request.BatchId);
            //var user = _userRepository.FindBy(accountBatch.CreatedBy);
            //var EmailAddress = user.Address.Email;

            //Email email = new Email();
            //email.EmailTo = EmailAddress.TrimEnd(';');
            //email.IsSent = false;
            //email.NumRetries = 0;

            //EmailTemplate emailTemplate = null;
            //emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "BatchReconciliation").SingleOrDefault();

            //EmailTemplateGenerator.BatchReconciliationNotification(email, accountBatch, emailTemplate);
            //_emailRepository.Add(email);
            //auditTracker.Created.Add(email);

            //_uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            //try
            //{

            //    _uow.Commit();

            //}
            //catch (Exception)
            //{
            //    throw;
            //}


            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "You successfully reconciled postpaid transactions in batch!", MessageType = Infrastructure.Messaging.MessageType.Success };
            return response;


        }


        public ViewSummaryPostpaidTransactionsResponse ViewSummaryPostpaidTransactions(ViewSummaryPostpaidTransactionsRequest request)
        {

            ViewSummaryPostpaidTransactionsResponse response = new ViewSummaryPostpaidTransactionsResponse();

            var membership = _membershipRepository.GetMembershipDetailById(request.SecurityUser.MembershipId);

            if (membership.MembershipAccountTypeId == MembershipAccountCategory.Regular)
            {
                request.LimitTo = 2;
            }
            else if (membership.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
            {
                request.LimitTo = 3;
            }


            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = PaymentQueryGenerator.CreateQueryForSummarisedPostpaidTransactions(request, _accountTransactionRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = PaymentQueryGenerator.CreateQueryForSummarisedPostpaidTransactions(request, _accountTransactionRepository, false);
            response.SummarisedCreditActivities = myquery2.ToList();
            return response;
        }
        public ViewSummaryPostpaidTransactionsResponse ViewSummaryPostpaidByBankTransaction(ViewSummaryPostpaidTransactionsRequest request)
        {

            ViewSummaryPostpaidTransactionsResponse response = new ViewSummaryPostpaidTransactionsResponse();

            var membership = _membershipRepository.GetMembershipDetailById(request.SecurityUser.MembershipId);

            if (membership.MembershipAccountTypeId == MembershipAccountCategory.Regular)
            {
                request.LimitTo = 2;
            }
            else if (membership.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
            {
                request.LimitTo = 3;
            }

            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = PaymentQueryGenerator.CreateQueryForSummarisedBankPostpaidTransactions(request, _accountTransactionRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = PaymentQueryGenerator.CreateQueryForSummarisedBankPostpaidTransactions(request, _accountTransactionRepository, false);
            response.SummarisedCreditActivitiesRepresentativeBank = myquery2.ToList();
            return response;
        }
        public ViewSummaryPostpaidTransactionsResponse ViewPostpaidTransactionDetails(ViewSummaryPostpaidTransactionsRequest request)
        {
            if (request.LimitTo == null)
            {
                var membership = _membershipRepository.GetMembershipDetailById(request.SecurityUser.MembershipId);

                if (membership.MembershipAccountTypeId == MembershipAccountCategory.Regular)
                {
                    request.LimitTo = 2;
                }
                else if (membership.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
                {
                    request.LimitTo = 3;
                }
            }

            ViewSummaryPostpaidTransactionsResponse response = new ViewSummaryPostpaidTransactionsResponse();
            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = PaymentQueryGenerator.CreateQueryForPostpaidTransactionDetials(request, _accountTransactionRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = PaymentQueryGenerator.CreateQueryForPostpaidTransactionDetials(request, _accountTransactionRepository, false);
            response.PostpaidTransactionDetails = myquery2.ToList();

            if (request.BatchId != null)
            {
                response.AccountBatchView = _accountBatchRepository.GetAccountBatchView((int)request.BatchId);
            }

            return response;
        }

        public ViewAccountBatchesResponse ViewAccountBatches(ViewAccountBatchesRequest request)
        {
            ViewAccountBatchesResponse response = new ViewAccountBatchesResponse();

            //Make sure that only regular clients or owners are allowed here
            if (!(request.SecurityUser.IsOwnerUser || request.SecurityUser.AccountType == 2))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //Make sure only administraotrs or finance officers or support are allowed here, include registrat
            if (!(request.SecurityUser.IsOwnerAdminRegistrySupport() || request.SecurityUser.IsClientAdministrator() || request.SecurityUser.IsFinanceOfficer()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }


            response = _accountBatchRepository.GetAccountBatches(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public ViewReconcilationsResponse ViewReconcilations(ViewReconcilationsRequest request)
        {

            ViewReconcilationsResponse response = new ViewReconcilationsResponse();

            if (request.PageIndex > 0)
            {
                var myquery = PaymentQueryGenerator.ViewAccountReconcilationQuery(request, _accountTransactionRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = PaymentQueryGenerator.ViewAccountReconcilationQuery(request, _accountTransactionRepository, false);
            response.AccountReconcilations = myquery2.ToList();
            return response;


        }

        public SubmitInterSwitchDetailsResponse CreateEditInterSwitchUserDetails(SubmitInterSwitchDetailsRequest request)
        {
            SubmitInterSwitchDetailsResponse response = new SubmitInterSwitchDetailsResponse();
            decimal? amountBfTopUp = null;
            if (request.InterSwitchUserView.IsTopUpPayment)
            {
                if (String.IsNullOrWhiteSpace(request.InterSwitchUserView.TopUpCode))
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "Please provide a topup PIN Code.", MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError };
                    return response;
                }

                PublicUserSecurityCode publicUserSecurityCode = _publicUserSecurityCodeRepository.GetDbSet().SingleOrDefault(s => s.SecurityCode == request.InterSwitchUserView.TopUpCode && s.IsActive == true);
                if (publicUserSecurityCode == null)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "The PIN Code you have entered is invalid.", MessageType = Infrastructure.Messaging.MessageType.Error };
                    return response;
                }

                if (publicUserSecurityCode.Balance <= 0)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "You don't have enough balance on your PIN Code.", MessageType = Infrastructure.Messaging.MessageType.Error };
                    return response;
                }

                amountBfTopUp = publicUserSecurityCode.Balance;
            }



            string txn_ref = null;
            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
            InterSwitchWebPayTransaction interSwitchWebPay = _interSwitchRepository.GetTransactionByRefNo(request.TransactionReference);
            if (interSwitchWebPay != null)
            {
                txn_ref = request.TransactionReference;
                interSwitchWebPay.CustName = request.InterSwitchUserView.Name;
                interSwitchWebPay.CustEmail = request.InterSwitchUserView.Email;
                interSwitchWebPay.CustPhone = request.InterSwitchUserView.Phone;
                interSwitchWebPay.BVN = request.InterSwitchUserView.BVN;
                interSwitchWebPay.Amount = Math.Round(request.InterSwitchUserView.Amount, 2);
                interSwitchWebPay.Quantity = (int)request.InterSwitchUserView.Quantity;
                interSwitchWebPay.TopUpCode = request.InterSwitchUserView.TopUpCode;
                interSwitchWebPay.IsTopUpPayment = request.InterSwitchUserView.IsTopUpPayment;
                interSwitchWebPay.BalanceBeforeTopUp = amountBfTopUp;
                interSwitchWebPay.IsAmountInput = request.InterSwitchUserView.UsePaymentAmount;
                auditTracker.Updated.Add(interSwitchWebPay);
            }
            else
            {
                txn_ref = "T" + InterSwitchHelper.GenerateCode();
                while (_interSwitchRepository.IsGeneratedReferenceCode(txn_ref)) { txn_ref = "T" + InterSwitchHelper.GenerateCode(); }
                string site_redirect_url = request.SiteRedirectUrl;
                int product_id = InterSwitchConfig.Product_Id;
                var pay_item_id = InterSwitchConfig.Pay_Item_Id;
                var mackey = InterSwitchConfig.InterSwitchMacKey;
                var stringToHash = txn_ref + product_id + pay_item_id + Math.Round(request.InterSwitchUserView.Amount * 100, 0) +
                                   site_redirect_url + mackey;
                var hash = InterSwitchHelper.GenerateApiHash(stringToHash);
                interSwitchWebPay = new InterSwitchWebPayTransaction()
                {
                    ProductId = product_id,
                    Amount = Math.Round(request.InterSwitchUserView.Amount, 2),
                    Quantity = (int)request.InterSwitchUserView.Quantity,
                    CurrencyCode = InterSwitchConfig.CurrencyCode,
                    SiteRedirectUrl = site_redirect_url,
                    TransactionReference = txn_ref,
                    Hash = hash,
                    PayItemId = pay_item_id,
                    SiteName = null,
                    CustId = null,
                    CustIdDesc = null,
                    CustName = request.InterSwitchUserView.Name,
                    CustEmail = request.InterSwitchUserView.Email,
                    CustPhone = request.InterSwitchUserView.Phone,
                    BVN = request.InterSwitchUserView.BVN,
                    CustNameDesc = null,
                    PayItemName = null,
                    LocalDteTime = DateTime.Now.ToShortTimeString(),
                    ResponseCode = false,
                    IsTopUpPayment = request.InterSwitchUserView.IsTopUpPayment,
                    IsAmountInput = request.InterSwitchUserView.UsePaymentAmount,
                    TopUpCode = request.InterSwitchUserView.TopUpCode,
                    BalanceBeforeTopUp = amountBfTopUp
                };
                auditTracker.Created.Add(interSwitchWebPay);
                _interSwitchRepository.Add(interSwitchWebPay);
            }
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            try
            {
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;


            }
            catch
            {

                throw;
            }
            response.TransactionRefNo = txn_ref;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public GetInterSwitchDetailsResponse GetInterSwitchDetails(GetInterSwitchDetailsRequest request)
        {
            GetInterSwitchDetailsResponse response = new GetInterSwitchDetailsResponse();
            InterSwitchWebPayTransaction interSwitchWebPay = _interSwitchRepository.GetTransactionByRefNo(request.TransactionRefNo);
            
            InterSwitchWebPayTransactionQueryResponse interSwitchWebPayTransactionQueryResponse = null;
            if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse != null)
            {
                interSwitchWebPayTransactionQueryResponse = interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse;
                if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode != InterSwitchConfig.UnknownTxnRefNo && !InterSwitchConfig.IsTempError(interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode))
                {
                    response.InterSwitchTransactionView = new InterSwitchTransactionView();
                    response.InterSwitchTransactionView.IsProcessed = true;
                    return response;
                }
            }

            response.InterSwitchTransactionView = new InterSwitchTransactionView()
                                                     {
                                                         Amount = interSwitchWebPay.Amount,
                                                         Currency = interSwitchWebPay.CurrencyCode,
                                                         cust_id = interSwitchWebPay.CustId,
                                                         Email = interSwitchWebPay.CustEmail,
                                                         BVN = interSwitchWebPay.BVN,
                                                         hash = interSwitchWebPay.Hash,
                                                         Id = null,
                                                         Name = interSwitchWebPay.CustName,
                                                         pay_item_id = interSwitchWebPay.PayItemId,
                                                         pay_item_name = interSwitchWebPay.PayItemName,
                                                         Phone = interSwitchWebPay.CustPhone,
                                                         site_redirect_url = interSwitchWebPay.SiteRedirectUrl,
                                                         txn_ref = interSwitchWebPay.TransactionReference,
                                                         product_id = interSwitchWebPay.ProductId,
                                                         Quantity = interSwitchWebPay.Quantity,
                                                         TopUpCode = interSwitchWebPay.TopUpCode,
                                                         IsTopUpPayment = interSwitchWebPay.IsTopUpPayment,
                                                         UsePaymentAmount = interSwitchWebPay.IsAmountInput,
                                                         BalanceBeforeTopUp = interSwitchWebPay.BalanceBeforeTopUp,
                                                         BalanceAfterTopUp = Math.Round(interSwitchWebPay.Amount, 2) + interSwitchWebPay.BalanceBeforeTopUp,
                                                         IsProcessed = false
                                                     };
            return response;
        }

        public InterSwitchApiQueryResponse GetInterSwitchApiQueryResult(InterSwitchApiQueryRequest request)
        {
            InterSwitchApiQueryResponse response = new InterSwitchApiQueryResponse();
            InterSwitchWebPayTransaction interSwitchWebPay = _interSwitchRepository.GetTransactionByRefNo(request.TransactionRefNo);

            var IsUpdate = false;
            if (interSwitchWebPay == null)
            {
                //incorrect transaction reference number
                return response;
            }
            //return response if it is an authorized transaction
            InterSwitchWebPayTransactionQueryResponse interSwitchWebPayTransactionQueryResponse = null;
            if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse != null)
            {
                IsUpdate = true;
                interSwitchWebPayTransactionQueryResponse = interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse;
                if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode != InterSwitchConfig.UnknownTxnRefNo && !InterSwitchConfig.IsTempError(interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode))
                {
                    response.ResponseCode = interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode;
                    return response;
                }
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var amount = Math.Round(interSwitchWebPay.Amount * 100, 0);
            var productId = interSwitchWebPay.ProductId;
            var mackey = InterSwitchConfig.InterSwitchMacKey;
            var url = string.Format("{0}productid={1}&transactionreference={2}&amount={3}",
                InterSwitchConfig.InterSwitchXmlApi, productId, request.TransactionRefNo, amount);
            var stringToHash = productId + request.TransactionRefNo + mackey;
            var hash = InterSwitchHelper.GenerateApiHash(stringToHash);
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = 30000;
            webRequest.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            webRequest.Headers.Add("Hash", hash);
            WebResponse webResponse = null;
            bool IsSuccess = true;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                IsSuccess = false;
            }

            //If there is no error accessing webservice

            if (IsSuccess)
            {

                Stream newStream = webResponse.GetResponseStream();
                StreamReader sr = new StreamReader(newStream);
                var result = sr.ReadToEnd();
                var apiQueryResult = JsonConvert.DeserializeObject<TransactionResponseView2>(result);


                //XML Serializer
                //var serializedResult = new XmlSerializer(typeof(TransactionResponseView));
                //var apiQueryResult = (TransactionResponseView)serializedResult.Deserialize(webResponse.GetResponseStream());

                if (interSwitchWebPayTransactionQueryResponse == null)
                {
                    interSwitchWebPayTransactionQueryResponse = new InterSwitchWebPayTransactionQueryResponse();
                }

                /*wrong call */
                apiQueryResult.ResponseCode = InterSwitchConfig.SuccessfulTransaction;
                apiQueryResult.Amount = 10020;
                /*******************/

                interSwitchWebPayTransactionQueryResponse.ResponseCode = apiQueryResult.ResponseCode;
                interSwitchWebPayTransactionQueryResponse.ResponseDesc =
                    apiQueryResult.ResponseDescription;
                interSwitchWebPayTransactionQueryResponse.Amount = Math.Round(apiQueryResult.Amount / 100, 0);
                interSwitchWebPayTransactionQueryResponse.CardNumber = apiQueryResult.CardNumber;
                interSwitchWebPayTransactionQueryResponse.MerchentRef = apiQueryResult.MerchantReference;
                interSwitchWebPayTransactionQueryResponse.PaymentRef = apiQueryResult.PaymentReference;
                interSwitchWebPayTransactionQueryResponse.RetreivalRef =
                    apiQueryResult.RetrievalReferenceNumber;
                //interSwitchWebPayTransactionQueryResponse.SplitAcconts = apiQueryResult.SplitAccounts;
                interSwitchWebPayTransactionQueryResponse.TransactionDate = DateTime.Now;
                interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse = interSwitchWebPayTransactionQueryResponse;

                if (!IsUpdate)
                {
                    auditTracker.Created.Add(interSwitchWebPayTransactionQueryResponse);
                }
                else
                {
                    auditTracker.Updated.Add(interSwitchWebPayTransactionQueryResponse);
                }


                interSwitchWebPay.ResponseCode = true;

                if (apiQueryResult.ResponseCode == InterSwitchConfig.SuccessfulTransaction)
                {
                    decimal AfterPaymentAmount = 0;
                    decimal PostpaidBalance = 0;
                    string EncryptedSecurityCode = null;
                    string EmailAddress = null;

                    PaymentAccountTransaction accountTransaction = new PaymentAccountTransaction();



                    Payment onlinePayment = new Payment()
                    {
                        Amount = (apiQueryResult.Amount / 100),
                        PaymentType = PaymentType.Normal,
                        IsActive = true,
                        Payee = interSwitchWebPay.CustName,
                        MembershipId = null,
                        T24TransactionNo = interSwitchWebPay.TransactionReference,
                        PaymentSource = PaymentSource.InterSwitchWebPay,
                        PaymentDate = AuditedDate,
                        InterSwitchWebPayTransaction = interSwitchWebPay
                    };
                    auditTracker.Created.Add(onlinePayment);

                    PublicUserSecurityCode publicUser = null;

                    publicUser = new PublicUserSecurityCode();
                    if (interSwitchWebPay.IsTopUpPayment)
                        publicUser = _publicUserSecurityCodeRepository.GetDbSet().SingleOrDefault(s => s.SecurityCode == interSwitchWebPay.TopUpCode && s.IsActive == true);
                    publicUser.SecurityCode = interSwitchWebPay.IsTopUpPayment ? interSwitchWebPay.TopUpCode : Util.GetNewValidationCode();
                    publicUser.PublicUserEmail = interSwitchWebPay.CustEmail.TrimNull();

                    if (!interSwitchWebPay.IsTopUpPayment)
                    {
                        auditTracker.Created.Add(publicUser);
                    }
                    else
                    {
                        auditTracker.Updated.Add(publicUser);
                    }

                    EncryptedSecurityCode = publicUser.SecurityCode.TrimNull();

                    publicUser.Balance = publicUser.Balance + (apiQueryResult.Amount / 100);

                    onlinePayment.PublicUserSecurityCode = publicUser;

                    AfterPaymentAmount = publicUser.Balance;
                    EmailAddress = publicUser.PublicUserEmail.TrimNull();

                    onlinePayment.PaymentNo = "PAY" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Payment);

                    accountTransaction.Amount = Math.Abs(onlinePayment.Amount);
                    accountTransaction.CreditOrDebit = CreditOrDebit.Credit;

                    accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.Payment;
                    accountTransaction.NewPrepaidBalanceAfterTransaction = AfterPaymentAmount;
                    accountTransaction.NewPostpaidBalanceAfterTransaction = PostpaidBalance;
                    accountTransaction.Payment = onlinePayment;
                    accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
                    auditTracker.Created.Add(accountTransaction);
                    _accountTransactionRepository.Add(accountTransaction);


                    if (EmailAddress != null)
                    {
                        Email email = new Email();
                        email.EmailTo = EmailAddress.TrimEnd(';');
                        email.IsSent = false;
                        email.NumRetries = 0;

                        EmailTemplate emailTemplate = null;
                        emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "WebPayPayment").SingleOrDefault();

                        EmailTemplateGenerator.InterSwitchPaymentNotification(email, onlinePayment, emailTemplate);

                        _emailRepository.Add(email);

                        auditTracker.Created.Add(email);
                    }
                    AuditAction = Model.FS.Enums.AuditAction.ReceivedUnRegisteredClientPayment;
                    AuditMessage = PaymentAuditMsgGenerator.PaymentDetails(onlinePayment);
                    accountTransaction.Narration = AuditMessage;
                    Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
                    _auditRepository.Add(audit);
                    auditTracker.Created.Add(audit);

                }
                response.ResponseCode = apiQueryResult.ResponseCode;
            }
            else
            {
                interSwitchWebPay.ResponseCode = false;
                response.IsConnectivityError = true;
            }

            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);

            auditTracker.Updated.Add(interSwitchWebPay);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            try
            {

                _uow.Commit();

            }
            catch (Exception)
            {
                throw;
            }



            return response;
        }

        public GetInterSwitchViewPaymentDetailsResponse ViewSummaryInterSwitchTransaction(GetInterSwitchViewPaymentDetailsRequest request)
        {
            GetInterSwitchViewPaymentDetailsResponse response = new GetInterSwitchViewPaymentDetailsResponse();
            InterSwitchWebPayTransaction interSwitchWebPay = _interSwitchRepository.GetTransactionByRefNo(request.TransactionRefNo);
            if (interSwitchWebPay != null)
            {

                response.TransactionResponseView = new TransactionResponseView();

                if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse != null)
                {
                    response.TransactionResponseView.ResponseCode =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode;
                    response.TransactionResponseView.ResponseDescription =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseDesc;
                    response.TransactionResponseView.Amount =
                        Math.Round(interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.Amount, 0);
                    response.TransactionResponseView.CardNumber =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.CardNumber;
                    response.TransactionResponseView.MerchantReference =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.PaymentRef;
                    response.TransactionResponseView.RetrievalReferenceNumber =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.RetreivalRef;
                    response.TransactionResponseView.SplitAccounts =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.SplitAcconts;
                    response.TransactionResponseView.TransactionDate =
                        interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.TransactionDate;
                }

                if (!String.IsNullOrWhiteSpace(interSwitchWebPay.CustEmail))
                {
                    response.HasEmail = true;
                }


                if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse != null
                    && interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode != InterSwitchConfig.UnknownTxnRefNo
                    && !InterSwitchConfig.IsTempError(interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode))
                {
                    response.IsAuthorized = true;
                }
                if (interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse != null && interSwitchWebPay.InterSwitchWebPayTransactionQueryResponse.ResponseCode == InterSwitchConfig.SuccessfulTransaction)
                {
                    var payment =
                        _paymentRepository.GetWebPayPayment(
                            interSwitchWebPay.Id);
                    response.PaymentId = payment.Id;
                    response.SecurityCode = payment.PublicUserSecurityCode.SecurityCode;
                }

                response.Payee = interSwitchWebPay.CustName;
                response.TransactionRefNo = request.TransactionRefNo;
            }
            else
            {
                response.Message = "Your transaction reference number is incorrect!";
            }


            return response;
        }

        public GetAllInterSwitchTransactionsResponse GetInterSwitchTransactions(GetAllInterSwitchTransactionsRequest request)
        {
            GetAllInterSwitchTransactionsResponse response = new GetAllInterSwitchTransactionsResponse();
            if (!request.SecurityUser.IsOwnerUser)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            if (request.PageIndex > 0)
            {
                var myquery =
                    PaymentQueryGenerator.CreateQueryForInterSwitchTransactions(request, _interSwitchRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }
            var myquery2 =
                    PaymentQueryGenerator.CreateQueryForInterSwitchTransactions(request, _interSwitchRepository, false);
            response.InterSwitchTransactionViews = myquery2.ToList();

            foreach (var item in response.InterSwitchTransactionViews)
            {
                if (!item.IsSuccess)
                {
                    item.Status = "Unsuccessful";
                }
                if (item.IsSuccess)
                {
                    item.Status = "Successful";
                }
                if (item.IsPending)
                {
                    item.Status = "Pending";
                }
                if (!(bool)item.IsProcessed)
                {
                    item.Status = "Incomplete";
                }

            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public GetAllDirectPayTransactionsResponse GetDirectPayTransactions(GetAllDirectPayTransactionsRequest request)
        {
            GetAllDirectPayTransactionsResponse response = new GetAllDirectPayTransactionsResponse();
            if (!request.SecurityUser.IsOwnerUser)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            if (request.PageIndex > 0)
            {
                var myquery =
                    PaymentQueryGenerator.CreateQueryForDirectPayTransactions(request, _interSwitchDirectPayRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }
            var myquery2 =
                    PaymentQueryGenerator.CreateQueryForDirectPayTransactions(request, _interSwitchDirectPayRepository, false);
            response.InterSwitchDetailViews = myquery2.ToList();

            foreach (var item in response.InterSwitchDetailViews)
            {
                if (!item.IsSuccess)
                {
                    item.Status = "Unsuccessful";
                }
                else
                {
                    item.Status = "Successful";
                }
                if (item.IsPending)
                {
                    item.Status = "Pending";
                }

            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public MakeDirectPayPaymentResponse MakePaymentDirectPayPayment(MakeDirectPayPaymentRequest request)
        {
            MakeDirectPayPaymentResponse response = new MakeDirectPayPaymentResponse();
            response.DirectPayResponseView = new DirectPayResponseView();
            string statusId = "00";
            int responseCode = 00;
            string responseMessage = "Successful";
            string TxRefId = request.DirectPayRequestView.SequenceNo;

            if (request.DirectPayRequestView.DestAccount == "080" &&
                request.DirectPayRequestView.Amount == 0 &&
                request.DirectPayRequestView.SequenceNo == "1" &&
                request.DirectPayRequestView.DealerNo == "Poll" &&
                request.DirectPayRequestView.Passsword == "Poll")
            {
                response.DirectPayResponseView = new DirectPayResponseView()
                {
                    DestAccount = "080",
                    Amount = 0,
                    ResponseCode = responseCode,
                    ResponseMessage = responseMessage,
                    StatusId = statusId,
                    StatusMessage = responseMessage,
                    DealerNo = "Poll",
                    TxRefId = TxRefId,
                    ValueToken = "Null",

                };
            }
            else
            {



                if (String.IsNullOrEmpty(request.DirectPayRequestView.DestAccount)
                    || request.DirectPayRequestView.Amount != null
                    || request.DirectPayRequestView.SequenceNo != null
                    || request.DirectPayRequestView.DestAccount != null
                    || String.IsNullOrEmpty(request.DirectPayRequestView.DealerNo)
                    || String.IsNullOrEmpty(request.DirectPayRequestView.Passsword))
                {
                    //Checking for when amount is less than search amount
                    if (request.DirectPayRequestView.Amount <= InterSwitchConfig.MinSearchAmt)
                    {
                        responseCode = 13;
                        statusId = "13";
                        responseMessage = "Invalid Amount";
                    }

                    if (request.DirectPayRequestView.Passsword != InterSwitchConfig.Password)
                    {
                        responseCode = 55;
                        statusId = "55";
                        responseMessage = "Verification Failed";
                    }
                    if (request.DirectPayRequestView.ProductCode != InterSwitchConfig.ProductCode)
                    {
                        responseCode = 40;
                        statusId = "40";
                        responseMessage = "Functionality not Supported";
                    }

                }
                else
                {
                    responseCode = 96;
                    statusId = "96";
                    responseMessage = "Transaction processing error";
                }

                var directPayTransactionCheck = _interSwitchDirectPayRepository.GetTransactionBySeqNo(request.DirectPayRequestView.SequenceNo);
                if (directPayTransactionCheck != null)
                {
                    response.DirectPayResponseView = directPayTransactionCheck.InterSwitchDirectPayTransactionQueryResponse.ConvertToDirectPayResponseView();
                    if (directPayTransactionCheck.InterSwitchDirectPayTransactionQueryResponse.StatusId == "00")
                    {
                        response.DirectPayResponseView.StatusId = "11";
                        response.DirectPayResponseView.ResponseCode = 11;
                    }
                    else
                    {
                        response.DirectPayResponseView.StatusId = "94";
                        response.DirectPayResponseView.ResponseCode = 94;
                    }
                    response.DirectPayResponseView.StatusMessage = "Duplicate Transaction";
                    response.DirectPayResponseView.ResponseMessage = "Duplicate Transaction";
                    response.DirectPayResponseView.ValueToken = "Null";
                    return response;
                }
                string SecurityCode = "Null";
                
                string DestAccount = request.DirectPayRequestView.DestAccount;

                var directPayTransaction = _interSwitchDirectPayRepository.GetTransactionByDestAcc(DestAccount);
                if (directPayTransaction == null)
                {
                    response.DirectPayResponseView.StatusId = "12";
                    response.DirectPayResponseView.ResponseCode = 12;
                    response.DirectPayResponseView.StatusMessage = "Invalid Payment Voucher Code";
                    response.DirectPayResponseView.ResponseMessage = "Invalid Payment Voucher Code";
                    response.DirectPayResponseView.ValueToken = "Null";
                    return response;
                }

                string DealerNo = request.DirectPayRequestView.DealerNo;
                decimal Amount = request.DirectPayRequestView.Amount;

                

                //directPayTransaction = new InterSwitchDirectPayTransaction();
               // directPayTransaction = request.DirectPayRequestView.ConvertToInterSwitchDirectPayTransaction();               
                InterSwitchDirectPayTransactionQueryResponse InterSwitchDirectPayTransactionQueryResponse = new InterSwitchDirectPayTransactionQueryResponse()
                {
                    StatusId = statusId,
                    StatusMessage = responseMessage,
                    TxRefId = TxRefId,
                    DealerNo = DealerNo,
                    DestAccount = DestAccount,
                    Amount = Amount,
                    ValueToken = SecurityCode,
                    ResponseCode = responseCode,
                    ResponseMessage = responseMessage
                };
                auditTracker.Created.Add(InterSwitchDirectPayTransactionQueryResponse);

                if (statusId == "00")
                {
                    decimal AfterPaymentAmount = 0;
                    decimal PostpaidBalance = 0;

                   
                    

                    PublicUserSecurityCode publicUser = null;
                    bool isTopUp = false;
                    InterSwitchDirectPayTransaction newDirectPayTransact = null;
                    if (directPayTransaction.InterSwitchDirectPayTransactionQueryResponse == null || directPayTransaction.InterSwitchDirectPayTransactionQueryResponse.StatusId != "00")
                    {                       
                        SecurityCode = Util.GetNewValidationCode();
                      
                        publicUser = new PublicUserSecurityCode();
                        publicUser.SecurityCode = SecurityCode;
                        publicUser.PublicUserEmail = directPayTransaction.CustEmail;
                        auditTracker.Created.Add(publicUser);

                        InterSwitchMapper.MapToInterSwitchDirectPayTransaction(directPayTransaction, request.DirectPayRequestView);
                        directPayTransaction.InterSwitchDirectPayTransactionQueryResponse = InterSwitchDirectPayTransactionQueryResponse;
                    }
                    else 
                    {
                        isTopUp = true;
                        newDirectPayTransact = new InterSwitchDirectPayTransaction();
                        newDirectPayTransact = request.DirectPayRequestView.ConvertToInterSwitchDirectPayTransaction();
                        newDirectPayTransact.CustName = directPayTransaction.CustName;
                        newDirectPayTransact.CustPhone = directPayTransaction.CustPhone;
                        newDirectPayTransact.CustEmail = directPayTransaction.CustEmail;
                        newDirectPayTransact.BVN = directPayTransaction.BVN;
                        newDirectPayTransact.IsTopUpPayment = true;
                        newDirectPayTransact.InterSwitchDirectPayTransactionQueryResponse = InterSwitchDirectPayTransactionQueryResponse;
                        auditTracker.Created.Add(newDirectPayTransact);

                        SecurityCode = directPayTransaction.InterSwitchDirectPayTransactionQueryResponse.ValueToken;
                        publicUser = _publicUserSecurityCodeRepository.GetDbSet().SingleOrDefault(s => s.SecurityCode == SecurityCode && s.IsActive == true && s.IsDeleted == false);
                        auditTracker.Updated.Add(publicUser);
                    }


                    InterSwitchDirectPayTransactionQueryResponse.ValueToken = SecurityCode;

                    publicUser.Balance = publicUser.Balance + Amount;
                    AfterPaymentAmount = publicUser.Balance;


                    PaymentAccountTransaction accountTransaction = new PaymentAccountTransaction();
                    Payment onlinePayment = new Payment()
                    {
                        Amount = Amount,
                        PaymentType = PaymentType.Normal,
                        IsActive = true,
                        Payee = directPayTransaction.CustName,
                        MembershipId = null,
                        T24TransactionNo = TxRefId,
                        PaymentSource = PaymentSource.InterswitchDirectPay,
                        PaymentDate = AuditedDate,                        
                        PublicUserSecurityCode = publicUser
                    };
                    if (isTopUp)
                    {
                        onlinePayment.InterSwitchDirectPayTransaction = newDirectPayTransact;
                    }
                    else
                    {
                        onlinePayment.InterSwitchDirectPayTransactionId = directPayTransaction.Id;
                    }

                    auditTracker.Created.Add(onlinePayment);

                    onlinePayment.PaymentNo = "PAY" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Payment);

                    accountTransaction.Amount = Math.Abs(onlinePayment.Amount);
                    accountTransaction.CreditOrDebit = CreditOrDebit.Credit;

                    accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.Payment;
                    accountTransaction.NewPrepaidBalanceAfterTransaction = AfterPaymentAmount;
                    accountTransaction.NewPostpaidBalanceAfterTransaction = PostpaidBalance;
                    accountTransaction.Payment = onlinePayment;
                    accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
                    auditTracker.Created.Add(accountTransaction);
                    _accountTransactionRepository.Add(accountTransaction);


                    if (directPayTransaction.CustEmail != null)
                    {
                        Email email = new Email();
                        email.EmailTo = directPayTransaction.CustEmail.TrimEnd(';');
                        email.IsSent = false;
                        email.NumRetries = 0;

                        EmailTemplate emailTemplate = null;
                        emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "DirectPayPayment").SingleOrDefault();

                        EmailTemplateGenerator.InterSwitchPaymentNotification(email, onlinePayment, emailTemplate);

                        _emailRepository.Add(email);

                        auditTracker.Created.Add(email);
                    }


                    AuditAction = Model.FS.Enums.AuditAction.ReceivedUnRegisteredClientPayment;
                    AuditMessage = PaymentAuditMsgGenerator.PaymentDetails(onlinePayment);
                    accountTransaction.Narration = AuditMessage;
                    Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
                    _auditRepository.Add(audit);
                    auditTracker.Created.Add(audit);
                }
                //else
                //{
                //    _interSwitchDirectPayRepository.Add(directPayTransaction);
                //}
                


                auditTracker.Updated.Add(directPayTransaction);

                if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);

                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

                response.DirectPayResponseView = InterSwitchDirectPayTransactionQueryResponse.ConvertToDirectPayResponseView();

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

        public MakeDirectPayReversalResponse MakeDirectPayReversal(MakeDirectPayReversalRequest request)
        {
            MakeDirectPayReversalResponse response = new MakeDirectPayReversalResponse();
            string statusId = "00";
            int responseCode = 00;
            string responseMessage = "Successful";
            string TxRefId = null;

            if (String.IsNullOrEmpty(request.DirectPayReversalRequestView.DestAccount)
                   || request.DirectPayReversalRequestView.Amount != null
                   || request.DirectPayReversalRequestView.SequenceNo != null
                   || String.IsNullOrEmpty(request.DirectPayReversalRequestView.DealerNo)
                   || String.IsNullOrEmpty(request.DirectPayReversalRequestView.Passsword))
            {

                if (request.DirectPayReversalRequestView.Passsword != InterSwitchConfig.Password)
                {
                    responseCode = 55;
                    statusId = "55";
                    responseMessage = "Verification Failed";
                }
                if (request.DirectPayReversalRequestView.ProductCode != InterSwitchConfig.ProductCode)
                {
                    responseCode = 40;
                    statusId = "40";
                    responseMessage = "Functionality not Supported";
                }

            }
            else
            {
                responseCode = 96;
                statusId = "96";
                responseMessage = "Transaction processing error";
            }

            string DealerNo = request.DirectPayReversalRequestView.DealerNo;
            string DestAccount = request.DirectPayReversalRequestView.DestAccount;
            decimal Amount = request.DirectPayReversalRequestView.Amount;

            var oldDirectPayTransaction = _interSwitchDirectPayRepository.GetTransactionBySeqNo(request.DirectPayReversalRequestView.SequenceNo);
            if (oldDirectPayTransaction == null)
            {
                response.DirectPayReversalResponseView = new DirectPayReversalResponseView();
                response.DirectPayReversalResponseView.StatusId = statusId;
                response.DirectPayReversalResponseView.ResponseCode = responseCode;
                response.DirectPayReversalResponseView.StatusMessage = responseMessage;
                response.DirectPayReversalResponseView.ResponseMessage = responseMessage;
                response.DirectPayReversalResponseView.Amount = Amount;
                response.DirectPayReversalResponseView.DealerNo = DealerNo;
                response.DirectPayReversalResponseView.DestAccount = DestAccount;
                response.DirectPayReversalResponseView.ValueToken = "Null";
                return response;
            }

            if (oldDirectPayTransaction.Amount != request.DirectPayReversalRequestView.Amount)
            {
                responseCode = 13;
                statusId = "13";
                responseMessage = "Invalid Amount";
            }

            var isReversed = _interSwitchDirectPayRepository.IsReversed(request.DirectPayReversalRequestView.SequenceNo);
            if (isReversed)
            {
                responseCode = 98;
                statusId = "98";
                responseMessage = "Transaction limit exceeded";
            }


            var InterSwitchDirectPayTransactionQueryResponse = new InterSwitchDirectPayTransactionQueryResponse()
            {
                StatusId = statusId,
                StatusMessage = responseMessage,
                TxRefId = TxRefId,
                DealerNo = DealerNo,
                DestAccount = DestAccount,
                Amount = Amount,
                ValueToken = "Null",
                ResponseCode = responseCode,
                ResponseMessage = responseMessage
            };
            auditTracker.Created.Add(InterSwitchDirectPayTransactionQueryResponse);

            var directPayTransaction = new InterSwitchDirectPayTransaction();
            directPayTransaction = request.DirectPayReversalRequestView.ConvertToInterSwitchDirectPayTransaction();
            directPayTransaction.CustName = oldDirectPayTransaction.CustName;
            directPayTransaction.CustEmail = oldDirectPayTransaction.CustEmail;
            directPayTransaction.CustPhone = oldDirectPayTransaction.CustPhone;
            directPayTransaction.BVN = oldDirectPayTransaction.BVN;
            directPayTransaction.InterSwitchDirectPayTransactionQueryResponse = InterSwitchDirectPayTransactionQueryResponse;
            directPayTransaction.IsReversal = true;
            directPayTransaction.IsProcessed = statusId == "00" ? true : false;
            auditTracker.Created.Add(directPayTransaction);


            if (statusId == "00")
            {
                decimal AfterPaymentAmount = 0;
                decimal PostpaidBalance = 0;

                PaymentAccountTransaction accountTransaction = new PaymentAccountTransaction();
                Payment reversedPayment = new Payment()
                {
                    Amount = Amount,
                    PaymentType = PaymentType.Reversal,
                    IsActive = true,
                    Payee = null,
                    MembershipId = null,
                    T24TransactionNo = TxRefId,
                    PaymentSource = PaymentSource.InterswitchDirectPay,
                    PaymentDate = AuditedDate,
                    InterSwitchDirectPayTransaction = directPayTransaction
                };
                auditTracker.Created.Add(reversedPayment);
                reversedPayment.PaymentNo = "PAY" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Payment);

                accountTransaction.Amount = Math.Abs(reversedPayment.Amount);
                accountTransaction.CreditOrDebit = CreditOrDebit.Debit;

                AfterPaymentAmount = reversedPayment.Amount;

                accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.ReversalPayment;
                accountTransaction.NewPrepaidBalanceAfterTransaction = AfterPaymentAmount;
                accountTransaction.NewPostpaidBalanceAfterTransaction = PostpaidBalance;
                accountTransaction.Payment = reversedPayment;
                accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
                auditTracker.Created.Add(accountTransaction);
                _accountTransactionRepository.Add(accountTransaction);


                Payment previousPayment = _paymentRepository.GetDirectPayPayment(oldDirectPayTransaction.Id);
                previousPayment.ReversedPayment = reversedPayment;
                auditTracker.Updated.Add(previousPayment);

                var publicUser = _publicUserSecurityCodeRepository.FindBy((int)previousPayment.PublicUserSecurityCodeId);
                publicUser.Balance = publicUser.Balance - Amount;
                auditTracker.Updated.Add(publicUser);

                AuditAction = Model.FS.Enums.AuditAction.ReceivedUnRegisteredClientPayment;
                AuditMessage = PaymentAuditMsgGenerator.PaymentDetails(reversedPayment);
                accountTransaction.Narration = AuditMessage;
                Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
                _auditRepository.Add(audit);
                auditTracker.Created.Add(audit);
            }
            else
            {
                _interSwitchDirectPayRepository.Add(directPayTransaction);
            }

            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);

            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            response.DirectPayReversalResponseView = InterSwitchDirectPayTransactionQueryResponse.ConvertToDirectPayReverseResponseView();

            try
            {
                _uow.Commit();

            }
            catch (Exception)
            {
                throw;
            }



            return response;
        }
        public ResponseBase UndoBatch(DeleteItemRequest request)
        {
            ResponseBase response = new ResponseBase();


            //Validate that only administrators can create new users
            if (!(request.SecurityUser.IsFinanceOfficer()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            int BatchId = _paymentRepository.UndoBatch(request.Id, request.SecurityUser.Id, request.UserIP, request.RequestUrl);

            if (BatchId == 0)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "Could not remove batch due to one or several errors!", MessageType = Infrastructure.Messaging.MessageType.Error };
                return response;
            }


            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "You have successfully removed this batch!", MessageType = Infrastructure.Messaging.MessageType.Success };
            return response;
        }
        //ViewSummaryPostpaidTransactions
        //ViewSummaryPostpaidByBankTransaction
        //ViewSummaryDetail
        //GenerateSettlement



        //GeneratePaymentReceipt
        //ViewPayments
        //ViewPayment
        //GetDataForPurchaseCredit
        //MakeSettlementPayment
        //Make
        public CreateDirectPayDetailsResponse CreateDirectPayDetails(CreateDirectPayDetailsRequest request)
        {
            CreateDirectPayDetailsResponse response = new CreateDirectPayDetailsResponse();

            InterSwitchDirectPayTransaction interSwitchDirectPay = new InterSwitchDirectPayTransaction();
            interSwitchDirectPay.CustName = request.InterSwitchUserView.Name;
            interSwitchDirectPay.CustPhone = request.InterSwitchUserView.Phone;
            interSwitchDirectPay.CustEmail = request.InterSwitchUserView.Email == null ? request.InterSwitchUserView.Email : request.InterSwitchUserView.Email.Trim();
            interSwitchDirectPay.BVN = request.InterSwitchUserView.BVN;
            var paymentVoucherCode = "P" + InterSwitchHelper.GenerateCode();
            while (_interSwitchDirectPayRepository.IsGeneratedPaymentCode(paymentVoucherCode)) { paymentVoucherCode = "P" + InterSwitchHelper.GenerateCode(); }
            interSwitchDirectPay.DestAccount = paymentVoucherCode;


            _interSwitchDirectPayRepository.Add(interSwitchDirectPay);
            if (request.SecurityUser == null) request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);


            auditTracker.Created.Add(interSwitchDirectPay);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);            

            try
            {
                _uow.Commit();

            }
            catch (Exception)
            {
                throw;
            }

            response.PublicVoucherCode = paymentVoucherCode;
            response.MessageInfo.MessageType = MessageType.Success;
            return response;
        }

        public GetPaymentVoucherResponse GetPaymentVoucher(GetPaymentVoucherRequest request)
        {
            GetPaymentVoucherResponse response = new GetPaymentVoucherResponse();
            var directPayTransaction = _interSwitchDirectPayRepository.GetTransactionByDestAcc(request.PaymentVoucherCode);

            var interSwitchDetails = new InterSwitchDetailsView()
            {
                Name = directPayTransaction.CustName,
                PaymentVoucherCode = directPayTransaction.DestAccount,
                Phone = directPayTransaction.CustPhone,
                Email = directPayTransaction.CustEmail,
                Amount = directPayTransaction.Amount
            };

            if (request.SecurityUser == null)
            {
                request.SecurityUser = new Infrastructure.Authentication.SecurityUser(Constants.PublicUser, "super", "", "", 2, 2, null, null, false, "", 1, false, 1);
                
            }

            LocalReport report = new LocalReport();
            report.ReportPath = report.ReportPath = Constants.GetReportPath + "Payment\\PaymentVoucher.rdlc"; ;
            ReportDataSource rdsFS = new ReportDataSource();
            ICollection<InterSwitchDetailsView> interSwitchDetailsViews = new List<InterSwitchDetailsView>();
            interSwitchDetailsViews.Add(interSwitchDetails);
            rdsFS.Name = "DataSet1";  
            rdsFS.Value = interSwitchDetailsViews;

            report.DataSources.Add(rdsFS);
            Byte[] mybytes = report.Render("PDF");


            response.AttachedFile = mybytes;
            response.AttachedFileName = "PaymentVoucher-PVC.pdf";
            response.AttachedFileSize = mybytes.Length.ToString();
            response.AttachedFileType = "application/pdf"; 
            
            return response;
        }

        public GetDirectPayTransactionDetailsResponse GetDirectPayTransactionDetails(GetDirectPayTransactionDetailsRequest request)
        {
            GetDirectPayTransactionDetailsResponse response = new GetDirectPayTransactionDetailsResponse();
            if (!request.SecurityUser.IsOwnerUser)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            var directpay = _interSwitchDirectPayRepository.FindBy(request.Id);
            response.InterSwitchDetailsView = new InterSwitchDetailsView()
            {
                Id = directpay.Id,
                PaymentVoucherCode = directpay.DestAccount,
                Name = directpay.CustName,
                Email = directpay.CustEmail,
                Phone = directpay.CustPhone,
                BVN = directpay.BVN,
                Amount = directpay.Amount,
                Currency = "Naira",
                TransactionDate = directpay.UpdatedOn ?? directpay.CreatedOn,
                IsTopUpPayment = directpay.IsTopUpPayment
            };
            response.MessageInfo.MessageType = MessageType.Success;
            return response;
        }
    }
}
