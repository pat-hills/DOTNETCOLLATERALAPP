using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Common.IRepository;
using CRL.Model.ModelViews;

using CRL.Service.BusinessServices;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.User.Request;
using CRL.Service.Messaging.User.Response;
using CRL.Service.QueryGenerator;
using CRL.Model.ModelViews;
using CRL.Service.Mappings.Membership;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model;
using System.Data.Entity;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS.Enums;
using CRL.Service.Messaging.Common.Request;
using CRL.Infrastructure.Authentication;
using CRL.Service.BusinessService;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelService;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Service.Common;
using CRL.Model.ModelViewMappers;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Infrastructure.Configuration;


namespace CRL.Service.Implementations
{
    public class UserService : ServiceBase, IUserService
    {
        //private readonly IInstitutionUnitRepository _institutionUnitRepository;
        //private readonly IInstitutionRepository _institutionRepository;
        //private readonly IUserRepository _userRepository;
        //private readonly IRoleRepository _roleRepository;
        //private readonly ILKCountryRepository _countryRepository;
        //private readonly ILKCountyRepository _countyRepository;   
        //private readonly ILKNationalityRepository _nationalityRepository;
        //private readonly IMembershipRepository _membershipRepository;
        //private readonly IEmailRepository _emailRepository;
        //private readonly IEmailTemplateRepository _emailTemplateRepository;
        //private readonly ISerialTrackerRepository _serialTrackerRepository;
        //private readonly IUnitOfWork _uow;
        //private readonly IWFCaseRepository _caseRepository;
        //private readonly IPasswordResetRequestRepository _passwordResetRequestRepository;
        //private readonly IAuditRepository _auditRepository;
        //private readonly IEmailUserAssignmentRepository _emailUserAssignmentRepository;
        //private readonly IWFWorkflowRepository _workflowRepository;
        //public AuditingTracker auditTracker { get; set; }
        //public DateTime AuditedDate { get; set; }
        //public String AuditMessage;
        //public AuditAction AuditAction { get; set; }

        //public UserService(
        //    IInstitutionUnitRepository institutionUnitRepository,
        //       IInstitutionRepository institutionRepository,
        //    IUserRepository userRepository, IRoleRepository roleRepository, ILKCountryRepository countryRepository, ILKCountyRepository countyRepository,
        //         IMembershipRepository membershipRepository,
        //    ILKNationalityRepository nationalityRepository, 
        //         IEmailRepository emailRepository,
        //  IEmailTemplateRepository emailTemplateRepository,
        //      ISerialTrackerRepository serialTrackerRepository,
        //    IPasswordResetRequestRepository passwordResetRequestRepository, IAuditRepository auditRepository,

        //    IUnitOfWork uow, IWFWorkflowRepository workflowRepository, IWFCaseRepository caseRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository)
        //{         
        //    _institutionUnitRepository = institutionUnitRepository;
        //     _institutionRepository = institutionRepository;
        //    _userRepository = userRepository;
        //    _roleRepository = roleRepository;
        //    _membershipRepository = membershipRepository;
        //    _countryRepository = countryRepository;
        //    _countyRepository = countyRepository;
        //    _nationalityRepository = nationalityRepository;
        //    _emailRepository = emailRepository;
        //    _emailTemplateRepository = emailTemplateRepository;
        //       _serialTrackerRepository=   serialTrackerRepository;
        //       _passwordResetRequestRepository = passwordResetRequestRepository;
        //       _auditRepository = auditRepository;
        //       _workflowRepository = workflowRepository;
        //       _caseRepository = caseRepository;
        //       _emailUserAssignmentRepository = emailUserAssignmentRepository;
        //       //_institutionTypesRepository = institutionTypesRepository;
        //       AuditedDate = DateTime.Now;
        //       auditTracker = new AuditingTracker();
        //    _uow = uow;


        //}

        public GetUserCreateResponse GetCreate(GetUserCreateRequest request)
        {
            GetUserCreateResponse response = new GetUserCreateResponse();

            //Validate the user security
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator() || request.SecurityUser.InstitutionId.HasValue))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //If we passed the institution id then logged in user must be from the Registry Administrator
            if (request.InstitutionId != null && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //If we passed the unit id the logged in user must be an Administrator
            if (request.InstitutionUnitId != null && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //Load lookups
            response.Countries = LookUpServiceModel.Countries(_countryRepository); //**caching may be necessary
            response.Countys = LookUpServiceModel.Countys(_countyRepository); //**caching may be necessary
            response.Nationalities = LookUpServiceModel.Nationalities(_nationalityRepository); //**caching maybe necessary
            if (request.InstitutionId != null || request.SecurityUser.InstitutionId != null)
            {
                response.InstitutionUnits = LookUpServiceModel.InstitutionUnits(_institutionUnitRepository, request.InstitutionId ?? (int)request.SecurityUser.InstitutionId);  //**Do not load for individuals
            }

            //Generate response
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;



        }
        public GetUserEditResponse GetEdit(GetUserEditRequest request)
        {
            //Security rrules apply here too
            GetUserEditResponse response = new GetUserEditResponse();

            if (request.UserView == null)
            {
                response.UserView = _userRepository.GetUserViewById((int)request.Id);
            }
            else
            {
                response.UserView = request.UserView;
            }

            //Validate user security
            if (request.SecurityUser.IsAdministrator() == false && request.SecurityUser.IsUnitAdministrator() == false)
            {
                if (request.SecurityUser.Id != request.Id)
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }
            else if (request.SecurityUser.IsOwnerAdministrator() == false && response.UserView.InstitutionId != request.SecurityUser.InstitutionId)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            else if (request.SecurityUser.IsAdministrator() == false && response.UserView.InstitutionUnitId != request.SecurityUser.InstitutionUnitId)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            if (response.UserView.InstitutionId == null)
            {
                response.TypeOfUser = Infrastructure.Enums.TypeOfUser.Individual;
            }
            else
            {
                if (response.UserView.MembershipView.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner)
                {
                    response.TypeOfUser = Infrastructure.Enums.TypeOfUser.CRLUser;
                }
                else
                {
                    response.TypeOfUser = Infrastructure.Enums.TypeOfUser.ClientUser;
                }

            }

            response.Countries = LookUpServiceModel.Countries(_countryRepository); //**caching may be necessary
            response.Countys = LookUpServiceModel.Countys(_countyRepository); //**caching may be necessary
            response.Nationalities = LookUpServiceModel.Nationalities(_nationalityRepository); //**caching maybe necessary
            response.InstitutionUnits = LookUpServiceModel.InstitutionUnits(_institutionUnitRepository, (int)response.UserView.InstitutionId);


            //Generate response
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;



        }
        public GetUserViewResponse GetView(GetUserViewRequest request)
        {
            //Security rrules apply here too
            GetUserViewResponse response = new GetUserViewResponse();
            response.UserView = _userRepository.GetUserViewById((int)request.Id);

            //Validate user security
            if (request.SecurityUser.IsAdministrator() == false && request.SecurityUser.IsUnitAdministrator() == false)
            {
                if (request.SecurityUser.Id != request.Id)
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }
            else if (request.SecurityUser.IsOwnerAdminRegistrySupport() == false && response.UserView.InstitutionId != request.SecurityUser.InstitutionId)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            else if (request.SecurityUser.IsAdministrator() == false && response.UserView.InstitutionUnitId != request.SecurityUser.InstitutionUnitId)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            if (response.UserView.InstitutionId == null)
            {
                response.TypeOfUser = Infrastructure.Enums.TypeOfUser.Individual;
            }
            else
            {
                if (response.UserView.MembershipView.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner)
                {
                    response.TypeOfUser = Infrastructure.Enums.TypeOfUser.CRLUser;
                }
                else
                {
                    response.TypeOfUser = Infrastructure.Enums.TypeOfUser.ClientUser;
                }

            }

            //Generate response
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;



        }
        //public GetDataForUserByEditModeResponse GetDataForCreateEditViewUser(GetDataForUserByEditModeRequest request)
        //{

        //    GetDataForUserByEditModeResponse response = new GetDataForUserByEditModeResponse();
        //    //Determine type of user
        //    if (request.Id != null && request.IgnoreLoadingUserView == false && (
        //          request.EditMode == Infrastructure.Enums.EditMode.Edit ||
        //           request.EditMode == Infrastructure.Enums.EditMode.View || request.EditMode == Infrastructure.Enums.EditMode.SummaryView))
        //    {
        //        response.UserView = GetUser((int)request.Id); //**If user is individual then we must provide the membership ifno except for summary view
        //    }

        //    int? UserInstitutionId;
        //    if (response.UserView != null)
        //    {
        //        UserInstitutionId = response.UserView.InstitutionId;
        //    }
        //    else
        //    {
        //        UserInstitutionId = request.UserInstitutionId;
        //    }

        //    if (UserInstitutionId == null)
        //    {
        //        response.TypeOfUser = Infrastructure.Enums.TypeOfUser.Individual;
        //    }
        //    else
        //    {
        //        if (_institutionRepository.GetInstitutionMembershipById((int)UserInstitutionId).Membership.MembershipTypeId == Model.Membership.Enums.MembershipCategory.Owner)
        //        {
        //            response.TypeOfUser = Infrastructure.Enums.TypeOfUser.CRLUser;
        //        }
        //        else
        //        {
        //            response.TypeOfUser = Infrastructure.Enums.TypeOfUser.ClientUser;
        //        }


        //    }

        //    if (request.EditMode == Infrastructure.Enums.EditMode.Create ||
        //                 request.EditMode == Infrastructure.Enums.EditMode.Edit)
        //    {
        //        //Are we getting for create then let's get the countries, let's get the nationalities, let's get the securing party types
        //        response.Countries = LookUpServiceModel.Countries(_countryRepository); //**caching may be necessary
        //        response.Countys  = LookUpServiceModel.Countys (_countyRepository); //**caching may be necessary
        //        response.Nationalities = LookUpServiceModel.Nationalities(_nationalityRepository); //**caching maybe necessary

        //        if (response.TypeOfUser != Infrastructure.Enums.TypeOfUser.Individual)
        //        {
        //            response.InstitutionUnits = LookUpServiceModel.InstitutionUnits(_institutionUnitRepository, (int)UserInstitutionId);  //**Do not load for individuals
        //        }
        //        if (response.TypeOfUser == Infrastructure.Enums.TypeOfUser.Individual)
        //        {
        //            response.RegularClientInstitutions = LookUpServiceModel.RegularInstitutions(_institutionRepository);
        //        }








        //    }

        //    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
        //    response.MessageInfo.Message = "Successfully added new client";
        //    return response;
        //}

        //private UserView GetUser(int Id, bool SummaryMode = false)
        //{
        //    UserView UserView;
        //    User user;
        //    if (SummaryMode)
        //    {
        //        user = _userRepository.GetDbSet().Where(item => item.Id == Id).SingleOrDefault();
        //        UserView = user.ConvertToUserView();
        //    }
        //    else
        //    {
        //        user = _userRepository.GetUserDetailById(Id); //**Add membership details
        //        UserView = user.ConvertToUserView();

        //        UserView.MembershipView = user.Membership.ConvertToMembershipView(_institutionRepository );
        //    }

        //    return UserView;

        //}


        //private UserView GetUser(User user, bool SummaryMode = false)
        //{


        //    if (SummaryMode)
        //    {
        //        user = _userRepository.GetDbSet().Where(item => item.Id == user.Id).SingleOrDefault();
        //    }
        //    else
        //    {
        //        user = _userRepository.GetUserDetailById(user.Id); //**Add membership details


        //    } 
        //   UserView UserView = user.ConvertToUserView();
        //   if (!SummaryMode)
        //   {
        //       user.Membership = _membershipRepository.GetMembershipDetailById((int)user.MembershipId);
        //       UserView.MembershipView = user.Membership.ConvertToMembershipView(_institutionRepository);
        //   }

        //    return UserView;
        //}
        public CreateEditUserResponse CreateUser(CreateEditUserRequest request) //For both institution and client, client may have account info
        {
            //Setup the institution
            CreateEditUserResponse response = new CreateEditUserResponse();

            //Validate that only administrators can create new users
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //validate that only Administrators can create Individual users
            else if (!request.SecurityUser.IsOwnerAdministrator() && request.UserView.InstitutionId == null)
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can create users from a different Institution
            else if (request.UserView.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can create users from a different unit 
            else if (request.UserView.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //Validate that the unit being created by the administrator or the unit administrator belogns to the correct institution
            if (request.SecurityUser.IsClientAdministrator() || request.SecurityUser.IsClientUnitAdministrator())
            {
                if (request.UserView.InstitutionUnitId != null)
                {
                    InstitutionUnit unit = _institutionUnitRepository.GetDbSet().Where(s => s.Id == request.UserView.InstitutionUnitId &&
                        s.InstitutionId == request.SecurityUser.InstitutionId && s.IsDeleted == false && s.IsActive == true).SingleOrDefault();

                    if (unit == null)
                    {
                        response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                        return response;
                    }

                }
            }
            //Validate that the institution unit being created belongs to the ins


            //Validate user login id and email
            ResponseBase response_VerifyUserLoginEmail = User.ValidateUserUniqueness(request.UserView.Username,
              request.UserView.Email, _userRepository);
            if (response_VerifyUserLoginEmail.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(response_VerifyUserLoginEmail); return response; }

            //Create user and map from userview entity
            User user = request.UserView.ConvertToUser();
            user.Password = request.UserView.Password;

            auditTracker.Created.Add(user);
            _userRepository.Add(user);

            //Create the membership service and setup the user account
            MembershipServiceModel ms = new MembershipServiceModel(_userRepository, _institutionRepository, _roleRepository, auditTracker, request.SecurityUser);
            ms.SetupUserAccount(user);

            if (request.NotifyUser != 2)
            {
                Email email = new Email();
                email.EmailTo = user.Address.Email;
                email.IsSent = false;
                email.NumRetries = 0;
                //Generate template
                EmailTemplate emailTemplate = null;
                bool createdByUserAdmin = true;
                string Password = request.NotifyUser == 0 ? request.UserView.Password : "";
                emailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "NotifyNewUser").SingleOrDefault();
                if (request.SecurityUser.IsOwnerUser)
                {
                    if (user.MembershipId != request.SecurityUser.MembershipId)
                    {
                        createdByUserAdmin = false;
                    }

                }
                EmailTemplateGenerator.NotifyUserOfNewAccountCreated(email, emailTemplate, user.Username, NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname)
                    , user.Address.Email, Password, request.UrlLink, createdByUserAdmin);

                _emailRepository.Add(email);
                auditTracker.Created.Add(email);

                //Assign email to the user
                EmailUserAssignment emailAssignment = new EmailUserAssignment();
                emailAssignment.AssignEmailToUser(email, user.Id);
                _emailUserAssignmentRepository.Add(emailAssignment);
                auditTracker.Created.Add(emailAssignment);
            }


            #region auditing and committing
            //Audititng Processing
            AuditAction = Model.FS.Enums.AuditAction.CreatedUser;
            AuditMessage = "User login id:" + user.Username;
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion

            //Return a success response
            response.UserView = request.UserView;
            response.UserView.Id = user.Id;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully added new user";
            return response;
        }
        public CreateEditUserResponse EditUser(CreateEditUserRequest request)
        {
            CreateEditUserResponse response = new CreateEditUserResponse();

            //Validate that only administrators can modify new users or users can modify themselves
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator() || request.SecurityUser.Id == request.UserView.Id))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //validate that only Registry Administrators can modify Individual users or Individual Users can modify themselves
            else if (!request.SecurityUser.IsOwnerAdministrator() && request.UserView.InstitutionId == null && (request.SecurityUser.Id != request.UserView.Id))
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can modify users from a different Institution
            else if (request.UserView.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can modify users from a different unit 
            else if (request.UserView.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            //Load the user we are editing
            User user = _userRepository.FindBy(request.UserView.Id);

            //Validate user login id and email
            ResponseBase response_VerifyUserLoginEmail = User.ValidateUserUniqueness(request.UserView.Id,
              request.UserView.Email, _userRepository);

            if (response_VerifyUserLoginEmail.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(response_VerifyUserLoginEmail); return response; }
            //Map from user view to user
            if (request.UserView.InstitutionUnitId != user.InstitutionUnitId)
            {
                user.BuildUserSettingOnLogin = true;
            }

            //If user has swtiched between global and unit user
            if (request.UserView.InstitutionUnitId == null && user.InstitutionUnitId != null ||
            (request.UserView.InstitutionUnitId != null && user.InstitutionUnitId == null))
            {
                //Remove all the roles of the user
                var removedUserRoles = user.Roles.ToList();
                foreach (var removed in removedUserRoles)
                {
                    user.Roles.Remove(removed);
                }
                user.BuildUserSettingOnLogin = true;

            }
            request.UserView.ConvertToUser(user); //Make sure that we do not change the membership info here
            auditTracker.Updated.Add(user);

            //Create new membership service class
            MembershipServiceModel ms = new MembershipServiceModel(_institutionRepository);
            ms.AuditTracker = auditTracker; //Sets it audit tracker


            if (user.InstitutionId == null)
            {
                AuditAction = AuditAction.ModifiedUser;
                AuditMessage = "User LoginId: " + user.Username;
            }
            else
            {
                AuditAction = AuditAction.ModifiedClientIndi;
                AuditMessage = "User LoginId: " + user.Membership.ClientCode;
            }

            #region auditing and committing
            //Audititng Processing

            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion

            //Return a success response
            response.UserView = request.UserView;
            response.UserView.Id = user.Id;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully edited user";
            return response;
        }
        public ResponseBase EditUserRolesList(EditUserRolesRequest request)
        {

            CreateEditUserResponse response = new CreateEditUserResponse();

            //Load for which user is credible for
            User user = _userRepository.FindBy(request.UserId);

            //Validate that only administrators can modify roles
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can modify roles of users from a different institution
            else if (user.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can modify roles of users from a different unit 
            else if (user.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that Administrators can't modify their own roles
            else if (user.Id == request.SecurityUser.Id)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                response.MessageInfo.Message = "You can't modify your own role(s).";
                return response;
            }

            //Prepare to remove roles that have been selected for removal from the user
            ICollection<Role> RolesToRemove = new List<Role>();
            bool UpdateUser = false;

            //We load the user's true rolesr
            foreach (var r in user.Roles)
            {
                if (request.RoleGridView.Where(s => s.Id == (int)r.Id).SingleOrDefault().isSelected == false)
                {
                    if (r.Name == "Administrator (Client)" || r.Name == "Administrator (Owner)")
                    {
                        //Check if we have users for these institutions
                        int count = _userRepository.GetDbSet().Where(s => s.Roles.Any(d => d.Name == "Administrator (Client)" || d.Name == "Administrator (Owner)")
                            && s.MembershipId == user.MembershipId && s.IsDeleted == false && s.IsActive == true).Count();

                        if (count < 2)
                        {
                            response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                            response.MessageInfo.Message = "Cannot remove Administrative role since this is the only active Administrator of this institution";//**Add the validation error here
                            return response;
                        }

                    }
                    RolesToRemove.Add(r);

                }

            }

            //Now we have the roles to remove, just remove them
            foreach (var r in RolesToRemove)
            {
                user.Roles.Remove(r);
                UpdateUser = true;
            }

            foreach (var r in request.RoleGridView.Where(s => s.isSelected == true))
            {

                if (user.Roles.Where(s => s.Id == (Roles)r.Id).SingleOrDefault() == null)
                {
                    Role newRole = _roleRepository.FindBy((Roles)r.Id);
                    user.Roles.Add(newRole);

                    UpdateUser = true;
                }
            }

            if (UpdateUser)
            {
                user.BuildUserSettingOnLogin = true;
                auditTracker.Updated.Add(user);


                #region auditing and committing
                //Audititng Processing
                AuditAction = AuditAction.ModifiedUserRoles;
                AuditMessage = MembershipAuditMsgGenerator.UserDetails(user);
                var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
                if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
                #endregion


            }

            response.UserView = _userRepository.GetUserViewById(user.Id);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully modified user";
            return response;
        }
        public ResponseBase RevokePaypointStatus(RequestBase request)
        {
            User user = _userRepository.FindBy(request.Id);
            user.isPayPointUser = false;


            AuditAction = AuditAction.DeactivateUserPaypoint;
            AuditMessage = "User LoginId: " + user.Username;


            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            return new ResponseBase("You successfully revoked the paypoint status of the user", Infrastructure.Messaging.MessageType.Success);


        }
        public ViewUsersResponse ViewUsers(ViewUsersRequest request)
        {
            ViewUsersResponse response = new ViewUsersResponse();

            if (request.InstitutionId != null && request.InstitutionId < 0)
            {
                throw new Exception("InstitutionId must be null or valid");
            }
            if (request.InstitutionUnitId != null && request.InstitutionUnitId < 0)
            {
                throw new Exception("Institution Unit Id must be null or valid");
            }

            //Validate that only administrators can view users
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator() || request.SecurityUser.InstitutionId.HasValue))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can view usersfrom other institutions
            else if ((request.InstitutionId != null) && (!request.SecurityUser.IsOwnerAdminRegistrySupport() && request.SecurityUser.InstitutionId != request.InstitutionId))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can view users
            else if (request.InstitutionUnitId != null && request.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //var roles = _roleRepository .GetDbSet().ToList();
            response = _userRepository.GetUserGrid(request);

            //foreach (var usr in response.UserGridView)
            //{
            //    foreach (var rl in usr.Roles)
            //    {
            //        usr.Roles =
            //    }
            //}
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;


        }
        public ViewUserRolesResponse ViewUserRoles(ViewUserRolesRequest request)
        {

            //Check request for edit so that we load only roles
            ViewUserRolesResponse response = new ViewUserRolesResponse();
            //Load for which user is credible for
            User user = _userRepository.GetDbSet().Where(s => s.Id == request.UserId).Single();
            //Validate that only administrators or users themselves can view their roles
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator() ||
                (request.LoadForEdit == false && (request.SecurityUser.Id == request.UserId || request.SecurityUser.InstitutionId == null))))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can modify users from a different Institution
            else if (user.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdminRegistrySupport())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can modify users from a different unit 
            else if (user.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            response.RoleGridView = _roleRepository.GetRolesGrid(request);

            //Based on roles that are selected maek them as selected or not
            if (request.LoadForEdit)
            {
                // We need to set those that are selected to selected
                foreach (var r in user.Roles)
                {
                    if (r.Users.Any(s => s.Id == request.UserId))
                    {
                        response.RoleGridView.Where(e => e.Id == (int)r.Id).SingleOrDefault().isSelected = true;

                    }
                }
            }


            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        public ResponseBase SetUserPaypointStatus(SetUserPaypointStatusRequest request)
        {
            ResponseBase response = new ResponseBase();
            User user = _userRepository.FindBy(request.Id);
            if (request.SetPaypointOnOrOff == 1)
            {
                user.isPayPointUser = true;
                AuditAction = AuditAction.ActivateUserPaypoint;
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "User has been set as paypoint", MessageType = Infrastructure.Messaging.MessageType.Success };


            }
            else
            {
                user.isPayPointUser = false;
                AuditAction = AuditAction.DeactivateUserPaypoint;
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "User paypoint status has been turned off!", MessageType = Infrastructure.Messaging.MessageType.Success };

            }


            AuditMessage = "User LoginId: " + user.Username;



            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();

            return response;


        }
        public ResponseBase ChangePasswordReset(ChangePasswordResetRequest request)
        {
            int configNumDays = Constants.ResetPasswordRequestExpiryDays;
            ResponseBase response = new ResponseBase();
            User user;
            string message = "";

            if (request.SecurityUser == null)
            {
                //Decrypt the password
                string code = Util.GetUrlDecode(request.RequestCode);
                PasswordResetRequest passwordRequest = _passwordResetRequestRepository.GetDbSet().Where(s => s.RequestCode == code).SingleOrDefault();
                if (passwordRequest == null)
                {
                    throw new Exception("Change password reset was not found"); ;//Respond with error
                }
                if (passwordRequest.RequestHandled)
                {
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                    response.MessageInfo.Message = "Password reset failed. This password reset request has already been handled.";
                    return response;
                }

                if (!passwordRequest.IsActive)
                {
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                    response.MessageInfo.Message = "Password reset failed. This password reset request has been deactivated because another password request which was also active has been handled";
                    return response;
                }
                //**Decryptr using the code
                if (passwordRequest.CreatedOn.AddDays(configNumDays) <= DateTime.Now)
                {
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                    response.MessageInfo.Message = "Password reset failed. This password reset request has expired";
                    return response;

                }

                //**Decryptr using the code
                if (String.IsNullOrWhiteSpace(request.Password))
                {
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                    response.MessageInfo.Message = "Password is not correct!";
                    return response;

                }

                if (passwordRequest.RequestCode == request.RequestCode)
                {
                    user = _userRepository.FindBy((int)passwordRequest.ResetUserId);
                    request.SecurityUser = new Infrastructure.Authentication.SecurityUser(user.Id, user.Username, "", "", 2, 2, null, null, false, "", 1, false, 1);

                    var passwordRequests = _passwordResetRequestRepository.GetDbSet().Where(s => s.ResetUserId == user.Id && s.RequestHandled == false && s.Id != passwordRequest.Id && s.IsActive == true && (DbFunctions.AddDays(s.CreatedOn, configNumDays)) > DateTime.Now).ToList();

                    foreach (var requestItem in passwordRequests)
                    {
                        var passRequest = _passwordResetRequestRepository.FindBy(requestItem.Id);
                        passRequest.IsActive = false;
                    }

                    passwordRequest.RequestHandled = true;
                }
                else
                {
                    throw new Exception("Request code was not equal to password");
                }

                message = "Your Password has been reset";
            }
            else
            {
                user = _userRepository.FindBy(request.SecurityUser.Id);
                message = "Your Password has been changed";
            }

            MembershipServiceModel ms = new MembershipServiceModel();
            user.Password = request.Password;
            ms.SetupUserPassword(user);


            Audit audit = new Audit("User Login: " + user.Username, request.RequestUrl, request.UserIP, AuditAction.ChangePasswordFromReset);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            try
            {
                _uow.Commit();
            }
            catch (Exception ex)
            {
                throw ex;

            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = message;



            return response;
        }
        private SecurityUser LoadSecurityUserFromUser(User usr)
        {
            //Load roles and convert them to string
            string rolenames = "";
            foreach (var rolename in usr.Roles.Where(s => s.IsActive == true && s.IsDeleted == false))
                rolenames += rolename.Name + "|";
            rolenames.Trim('|');

            //If we are are here then it meas everything is okay
            //We need to put all things in db before logging in
            SecurityUser user = new SecurityUser(usr.Id,
                usr.Username,
                 NameHelper.GetFullName(usr.FirstName, usr.MiddleName, usr.Surname)
                 , usr.Address.Email,
                 (int)usr.MembershipId,
                 usr.InstitutionId,
                 usr.InstitutionUnitId, rolenames, usr.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner, usr.Membership.ClientCode, usr.Membership.MajorRoleIsSecuredPartyOrAgent, usr.isPayPointUser, (int)usr.Membership.MembershipAccountTypeId);

            //if (usr.LastPasswordChangeDate is required)
            //set the usr.PasswordReuired to true
            return user;
        }
        public ForcedPasswordChangeResponse ForcePasswordChange(ForcePasswordChangeRequest request)
        {
            ForcedPasswordChangeResponse response = new ForcedPasswordChangeResponse();
            //Lets check if the password reset is true and that the 
            User user = null;
            if (request.IsNotForced)
            {
                user = _userRepository.GetDbSet().Where(s => s.Id == request.SecurityUser.Id && s.IsDeleted == false && s.IsActive == true).SingleOrDefault();

                if (SecurityHelper.HashPassword(request.OldPassword, user.PasswordSalt, new SHA256Managed()) != user.Password)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error };
                    response.MessageInfo.Message = "Your old password you entered is the same as the current password. Please enter a correct password.";
                    return response;
                }
            }
            else
            {
                user = _userRepository.GetDbSet().Where(s => s.Id == request.Id && s.IsDeleted == false && s.IsActive == true && s.ResetPasswordNextLogin == true && s.ResetPasswordNextLoginCode == request.PasswordResetCode).SingleOrDefault();
            }

            if (user == null)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            if (SecurityHelper.HashPassword(request.Password, user.PasswordSalt, new SHA256Managed()) == user.Password)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error };
                response.MessageInfo.Message = "The password you entered is the same as the previous password. Please enter a new one.";
                return response;
            }

            user.Password = request.Password;
            MembershipServiceModel ms = new MembershipServiceModel();
            ms.SetupUserPassword(user);
            user.ResetPasswordNextLoginCode = null;
            user.ResetPasswordNextLogin = false;

            Audit audit = new Audit("User Login Id: " + user.Username, request.RequestUrl, request.UserIP, AuditAction.ForcedChangePassword);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            try
            {
                _uow.Commit();
            }
            catch
            {
                throw;

            }
            if (!request.IsNotForced)
            {
                response.User = LoadSecurityUserFromUser(user);
            }
            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "Password changed successfully!", MessageType = Infrastructure.Messaging.MessageType.Success };
            return response;

        }
        public ResponseBase ResetPassword(ResetPasswordRequest request)
        {
            int configNumDays = Constants.ResetPasswordRequestExpiryDays;
            //Get the user with this email
            ResponseBase response = new ResponseBase();
            //Get the audited date
            DateTime AuditedDate = DateTime.Now;
            //Set up the AuditingTracker for auditing entities
            AuditingTracker auditTracker = new AuditingTracker();
            bool failed = false; string Message = "";
            User user = _userRepository.GetDbSet().Where(s => s.IsDeleted == false && s.Address.Email.ToLower() == request.Email.ToLower().Trim()).SingleOrDefault();

            if (user == null)
            {
                failed = true;
                Message = "User not found";
            }
            else
            {
                if (user.Membership.IsActive != true) { failed = true; Message = "User's client account not activated"; }


                if (user.IsLockedOut && (Constants.EnableUnlockAfterUnlockMinutes == false || (Constants.EnableUnlockAfterUnlockMinutes == true && user.LastLockedOutDate > DateTime.Now)))
                {
                    failed = true; Message = "User's account is locked!";
                    if (Constants.EnableUnlockAfterUnlockMinutes)
                        Message += " Please wait for " + Convert.ToInt32((user.LastLockedOutDate.Value - DateTime.Now).TotalMinutes) + " minutes for it to be automatically unlocked";
                    else
                        Message += "Please contact the Administrator of your system.";

                }
            }




            //Process other failures

            if (failed)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error, Message = Message };
                return response;
            }

            PasswordResetRequest passwordRequest = null;
            //passwordRequest = _passwordResetRequestRepository.GetDbSet().Where(s => s.ResetUserId == user.Id && s.RequestHandled == false && (DbFunctions.AddDays(s.CreatedOn, configNumDays)) < DateTime.Now).OrderByDescending(s=>s.Id).FirstOrDefault();
            //Passed we can set password
            if (passwordRequest == null)
            {
                passwordRequest = new PasswordResetRequest();
                passwordRequest.RequestCode = Util.GetNewValidationCode();
                passwordRequest.ResetUserId = user.Id;
                passwordRequest.CreatedBy = Constants.PublicUser;
                passwordRequest.CreatedOn = DateTime.Now;


                _passwordResetRequestRepository.Add(passwordRequest);
                //Send email
                Email ResetPasswordEmail = new Email();
                ResetPasswordEmail.IsSent = false;
                ResetPasswordEmail.NumRetries = 0;
                EmailTemplate ResetPasswordEmailTemplate;
                //Generate template
                string EncryptedPasswordResetCode = passwordRequest.RequestCode;
                request.UrlLink = request.UrlLink.Replace("RESETCODE", Util.GetUrlEncode(EncryptedPasswordResetCode));
                ResetPasswordEmailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "ResetPassword").SingleOrDefault();
                ResetPasswordEmail.EmailTo = user.Address.Email;  //Workflowemails not added but will be part of this list
                EmailTemplateGenerator.ResetPasswordEmail(ResetPasswordEmail, user, request.UrlLink, configNumDays.ToString(), ResetPasswordEmailTemplate);
                _emailRepository.Add(ResetPasswordEmail);
                auditTracker.Created.Add(ResetPasswordEmail);

                Audit audit = new Audit("User Login: " + user.Username, request.RequestUrl, request.UserIP, AuditAction.SubmitPasswordReset);
                audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
                _auditRepository.Add(audit);
                auditTracker.Created.Add(audit);
                _uow.AuditEntities(Constants.PublicUser, AuditedDate, auditTracker);


                try
                {
                    _uow.Commit();
                }

                catch
                {

                    throw;

                }
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Success, Message = "Password reset has been sent" };
            }
            else
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Info, Message = "A request for password change has already been request" };
            }

            return response;






        }
        public ResponseBase ChangeUserStatus(ChangeItemStatusRequest request)
        {
            ResponseBase response = new ResponseBase();

            //Users cannot delete themselves
            if (request.Id == request.SecurityUser.Id)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "You cannot deactivate or activate yourself";
                return response;
            }

            User user = _userRepository.FindBy(request.Id);

            //Validate that only administrators can delete users
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //validate that only Registry Administrators can delete  Individual users 
            else if (!request.SecurityUser.IsOwnerAdministrator() && user.InstitutionId == null)
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can delete users from a different institution
            else if (user.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can delete users from a different unit 
            else if (user.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            if (request.Activate == false)
            {

                if (user.Roles.Any(s => s.Name == "Administrator (Client)" || s.Name == "Administrator (Owner)") && user.Institution != null)
                {
                    int NumOtherAdministrator = _userRepository.GetDbSet().Where(s => s.MembershipId == user.MembershipId && s.Id != user.Id && s.IsActive == true && s.IsDeleted == false && s.Roles.Any(d => d.Name == "Administrator (Client)" || d.Name == "Administrator (Owner)")).Count();

                    if (NumOtherAdministrator < 1)
                    {
                        response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                        response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                        response.MessageInfo.Message = "Cannot remove user since this is the only active Administrator of this institution";//**Add the validation error here
                        return response;

                    }
                }
            }
            //**Check to see if we are not reassigning stratus again

            user.IsActive = request.Activate;

            //Set up the AuditingTracker for auditing entities
            auditTracker.Updated.Add(user);
            if (request.Activate == true)
                AuditAction = AuditAction.ActivatedUser;
            else
                AuditAction = AuditAction.DeactivatedUser;
            Audit audit = new Audit("User login Id:" + user.Id, request.RequestUrl, request.UserIP, AuditAction);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);

            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            return new ResponseBase(request.Activate == true ? "User activated" : "User deactivated", Infrastructure.Messaging.MessageType.Success);
        }
        public ResponseBase DeleteUser(DeleteItemRequest request)
        {
            ResponseBase response = new ResponseBase();

            if (!Constants.EnableDeletionOfUsers)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "Cannot delete user because deletion of users configuration is turned off under configurations";
                return response;
            }

            //Users cannot delete themselves
            if (request.Id == request.SecurityUser.Id)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "You cannot delete yourself";
                return response;
            }




            User user = _userRepository.FindBy(request.Id);

            //Validate that only administrators can delete users
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //validate that only Registry Administrators can delete  Individual users 
            else if (!request.SecurityUser.IsOwnerAdministrator() && user.InstitutionId == null)
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can delete users from a different institution
            else if (user.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can delete users from a different unit 
            else if (user.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //**Check to see if we are not reassigning stratus again

            //Check if we have users for these institutions
            if (user.Roles.Any(s => s.Name == "Administrator (Client)" || s.Name == "Administrator (Owner)") && user.Institution != null)
            {
                int count = _userRepository.GetDbSet().Where(s => s.Roles.Any(d => d.Name == "Administrator (Client)" || d.Name == "Administrator (Owner)")
                    && s.MembershipId == user.MembershipId && s.IsDeleted == false && s.IsActive == true).Count();

                if (count < 2)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                    response.MessageInfo.Message = "Cannot remove Administrative user since this is the only active Administrator of this institution";//**Add the validation error here
                    return response;
                }
            }

            user.IsDeleted = true;
            Audit audit = new Audit("Client Code:" + user.Membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction.DeletedUser);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            response.MessageInfo.Message = "User account deleted!";
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        public ResponseBase ProcessPayPointUsersRequest(SubmitPayPointUsersRequest request)
        {
            ResponseBase response = new ResponseBase();
            WFCasePaypointUsersAssignment _case = null;


            var paypointUsers = _userRepository.GetDbSet().Where(s => request.PaypointUsersIds.Contains(s.Id));


            if (request.RequestMode == RequestMode.Submit)
            {
                #region Process Workflow
                WorkflowServiceModel wfServiceModel = new WorkflowServiceModel(_emailTemplateRepository, _workflowRepository, _emailRepository, _emailUserAssignmentRepository, _userRepository, _caseRepository, auditTracker, request.SecurityUser);
                _case = (WFCasePaypointUsersAssignment)wfServiceModel.InitialiseCase(6, WorkflowRequestType.PaypointUserAssigment, request.Comment, null);
                _case.CaseTitle = "Authorization of Pay Point Users";
                _case.CaseType = Model.WorkflowEngine.Enums.WorkflowRequestType.PaypointUserAssigment;
                _case.TaskType = WFTaskType.PaypointUserAssigment;
                _case.Users = paypointUsers.ToList();
                _caseRepository.Add(_case);
                wfServiceModel.ProcessCase(WFTaskType.PaypointUserAssigment, request.SecurityUser.ClientCode, 1);
                #endregion

                Institution institution = _institutionRepository.GetDbSet().Where(s => s.Membership.Id == request.SecurityUser.MembershipId).Single();
                AuditAction = Model.FS.Enums.AuditAction.SubmitPayPointUsers;
                Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
                audit.Message = MembershipAuditMsgGenerator.PayPointUserAssignmentDetails(institution, paypointUsers.Count());
                _auditRepository.Add(audit);
                auditTracker.Created.Add(audit);
                auditTracker.Created.Add(_case);
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
                try
                {
                    _uow.Commit();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Success, Message = "Your request has been submitted to the National Collateral Registry of Nigeria for approval" };
                return response;
            }



            else
            {


                return response;
            }
        }

        public ResponseBase UnlockUser(UnlockItemRequest request)
        {
            ResponseBase response = new ResponseBase();

            User user = _userRepository.FindBy(request.Id);

            //Validate that only administrators can unlock users
            if (!(request.SecurityUser.IsAdministrator() || request.SecurityUser.IsUnitAdministrator()))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //validate that only Registry Administrators can unlock  Individual users 
            else if (!request.SecurityUser.IsOwnerAdministrator() && user.InstitutionId == null)
            {

                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Registry administrators can unlock users from a different institution
            else if (user.InstitutionId != request.SecurityUser.InstitutionId && !request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //Validate that only Administrators can unlock users from a different unit 
            else if (user.InstitutionUnitId != request.SecurityUser.InstitutionUnitId && !request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            //**Check to see if we are not reassigning status again


            user.IsLockedOut = false;
            user.LastLockedOutDate = null;
            Audit audit = new Audit("Client Code:" + user.Membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction.ActivatedUser);
            audit.Message = MembershipAuditMsgGenerator.UserDetails(user);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();

            response.MessageInfo.Message = "User account unlocked!";
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;

        }

        public ResponseBase CheckPasswordResetCode(CheckPasswordResetCodeRequest request)
        {
            ResponseBase response = new ResponseBase();

            int configNumDays = Constants.ResetPasswordRequestExpiryDays;

            PasswordResetRequest passwordRequest = _passwordResetRequestRepository.GetDbSet().Where(s => s.RequestCode == request.RequestCode).SingleOrDefault();
            if (passwordRequest == null)
            {
                throw new Exception("Change password reset was not found"); ;//Respond with error
            }
            else if (passwordRequest.RequestHandled)
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                response.MessageInfo.Message = "Password reset failed. This password reset request has already been handled";
            }
            else if (!passwordRequest.IsActive)
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                response.MessageInfo.Message = "Password reset failed. This password reset request has been deactivated because another password request which was also active has been handled";
            }
            else if (passwordRequest.CreatedOn.AddDays(configNumDays) <= DateTime.Now)
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Error;
                response.MessageInfo.Message = "Password reset failed. This password reset request has expired";
            }
            else
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
                response.MessageInfo.Message = "Request code is valid";
            }

            return response;
        }
    }
}
