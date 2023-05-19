using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Enums;


using CRL.Service.Interfaces;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Service.Views.Memberships;
using CRL.Service.Mappings.Membership;
using CRL.Service.QueryGenerator;
using CRL.Infrastructure.Authentication;
using System.Security.Cryptography;
using CRL.Service.Common;
using CRL.Model.FS.IRepository;
using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Institution.Response;
using CRL.Service.BusinessServices;
using CRL.Model.Common.IRepository;
using CRL.Infrastructure.Domain;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Model.ModelViews;
using CRL.Service.Messaging.Common.Request;
using CRL.Model;
using CRL.Model.FS.Enums;
using System.Data.Entity;
using CRL.Service.BusinessService;
using System.Data.Entity.Infrastructure;

using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViewValidators.MembershipViews;
using CRL.Model.ModelCaching;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;
using CRL.Infrastructure.Configuration;

namespace CRL.Service.Implementations
{
    public class InstitutionService : ServiceBase, IInstitutionService
    {
        public InstitutionService()
            : base()
        {

        }

        public GetCreateInstitutionResponse GetCreate(GetCreateInstitutionRequest request)
        {
            GetCreateInstitutionResponse response = new GetCreateInstitutionResponse();

            if (!request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            

            response.Countries = CachedData.GetLookUpData(CachedData.CACHE_COUNTRIES) ?? LookUpServiceHelper.Countries(_countryRepository);
            response.Countys = CachedData.GetLookUpData(CachedData.CACHE_COUNTYS) ?? LookUpServiceHelper.Countys(_countyRepository); //**caching may be necessary
            response.LGAs = CachedData.GetLookUpData(CachedData.CACHE_LGAS) ?? LookUpServiceHelper.LGAsLK(_lkgaRepository);
          
            
            response.Nationalities = CachedData.GetLookUpData(CachedData.CACHE_NATIONALITIES) ?? LookUpServiceHelper.Nationalities(_nationalityRepository); //**caching maybe necessary
            response.RegularClientInstitutions = LookUpServiceHelper.RegularInstitutions(_institutionRepository);
            response.SecuringPartyTypes = CachedData.GetLookUpData(CachedData.CACHE_SECURINGPARTYTYPES) ?? LookUpServiceHelper.SecuringPartyTypes(_securingPartyCategoryRepository);          

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;            
            return response;
        }
        public GetEditInstitutionResponse GetEdit(GetEditInstitutionRequest request)
        {
            GetEditInstitutionResponse response = new GetEditInstitutionResponse();

            if (!request.SecurityUser.IsOwnerAdministrator() &&
                !(request .SecurityUser .IsClientAdministrator () && request .SecurityUser.InstitutionId == request .Id ))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }


            response.Countries = CachedData.GetLookUpData(CachedData.CACHE_COUNTRIES) ?? LookUpServiceHelper.Countries(_countryRepository);
            response.Countys = CachedData.GetLookUpData(CachedData.CACHE_COUNTYS) ?? LookUpServiceHelper.Countys(_countyRepository); //**caching may be necessary
            response.Nationalities = CachedData.GetLookUpData(CachedData.CACHE_NATIONALITIES) ?? LookUpServiceHelper.Nationalities(_nationalityRepository); //**caching maybe necessary
            response.LGAs = CachedData.GetLookUpData(CachedData.CACHE_LGAS) ?? LookUpServiceHelper.LGAsLK(_lkgaRepository);
            response.RegularClientInstitutions = LookUpServiceHelper.RegularInstitutions(_institutionRepository);
            response.SecuringPartyTypes = CachedData.GetLookUpData(CachedData.CACHE_SECURINGPARTYTYPES) ?? LookUpServiceHelper.SecuringPartyTypes(_securingPartyCategoryRepository);

            //Load institution view if we have an Id provided, or if we are have not specify to ignore it or if we are not in create mode
            if (request.IgnoreLoadingInstitutionView == false)
            {
                response.InstitutionView = _institutionRepository.InstitutionMembershipViewById((int)request.Id);

            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        public GetViewInstitutionResponse GetView(GetViewInstitutionRequest request)
        {
            GetViewInstitutionResponse response = new GetViewInstitutionResponse();

            if (!request.SecurityUser.IsOwnerAdminRegistrySupport () &&
                !(request.SecurityUser.InstitutionId == request.Id))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            response.InstitutionView = _institutionRepository.InstitutionMembershipViewById((int)request.Id);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        public ViewClientInstitutionsResponse ViewClientInstitutions(ViewClientInstitutionsRequest request)
        {
            ViewClientInstitutionsResponse response  =  new ViewClientInstitutionsResponse ();
            if (!request.SecurityUser.IsOwnerAdminRegistrySupport())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            response= _institutionRepository.InstitutionGridView(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        //========================================================================================
        public CreateEditClientInstitutionResponse CreateClientInstitution(CreateEditClientInstitutionRequest request)
        {
            CreateEditClientInstitutionResponse response = new CreateEditClientInstitutionResponse();

            if (!request.SecurityUser.IsOwnerAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }            
            //Check if business code is unique
            if (_institutionRepository.GetDbSet().Where(s => s.CompanyNo.ToLower() == request.InstitutionView.CompanyNo.ToLower()
                && s.IsDeleted == false).Count() > 0)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                response.MessageInfo.Message = "The company no of the institution is already in used by another user account";//**Add the validation error here
                return response;
            }

            //Validate the InstitutionView
            InstitutionViewValidator institutionValidator = new InstitutionViewValidator();
            var result = institutionValidator.Validate(request.InstitutionView);
            if (!result.IsValid)  //If there are errors then we need to map them to the ModelState
            {
                foreach (var failure in result.Errors)
                {
                    String propertyname = failure.PropertyName;
                    response.ValidationErrors.Add(new ValidationError { ErrorMessage = failure.ErrorMessage, PropertyName = failure.PropertyName });
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                    response.MessageInfo.Message = "Business validation errors detected!";//**Add the validation error here

                }
                return response;
            }

            //Setup the institution
            Institution institution = request.InstitutionView.ConvertToInstitution();

            auditTracker.Created.Add(institution); //Set the institution as audited

            //Create new membership service class
            MembershipServiceModel ms = new MembershipServiceModel(_institutionRepository);
            ms.AuditTracker = auditTracker; //Sets it audit tracker
            ms.CreateClientInstitutionMembership(institution, request.InstitutionView.MembershipView.AccountNumber.TrimNull(), request.InstitutionView.MembershipView.RepresentativeMembershipId,
                request.InstitutionView.MembershipView.MajorRoleIsSecuredPartyOrAgent, true);

            institution.Membership.ClientCode = "MCC" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.ClientCode);
            institution.Membership.isIndividualOrLegalEntity = 2;


            #region auditing and committing
            //Audititng Processing
            AuditAction = AuditAction.CreatedClientLegal;
            AuditMessage = MembershipAuditMsgGenerator.InstitutionDetails(institution);
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion          


            response.InstitutionView = request.InstitutionView;
            response.InstitutionView.Id = institution.Id;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully added new client";
            return response;
        }
        public CreateEditClientInstitutionResponse EditClientInstitution(CreateEditClientInstitutionRequest request)
        {
            CreateEditClientInstitutionResponse response = new CreateEditClientInstitutionResponse();

            if (!request.SecurityUser.IsOwnerAdministrator() &&
             !(request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId == request.Id))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }


            
            
        


            Institution institution = _institutionRepository.GetInstitutionDetailById(request.InstitutionView.Id);
            institution.Membership = _membershipRepository.GetMembershipDetailById((int)institution.MembershipId);

            if (institution.CompanyNo.Trim() != request.InstitutionView.CompanyNo.Trim())
            {
                //Check if business code is unique
                if (_institutionRepository.GetDbSet().Where(s => s.CompanyNo.ToLower() == request.InstitutionView.CompanyNo.ToLower() && 
                    s.IsDeleted == false && institution.Id != request.InstitutionView.Id).Count() > 0)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                    response.MessageInfo.Message = "The company no of the institution is already in used by another user account";//**Add the validation error here
                    return response;
                }
            }

            institution.SecuringPartyTypeId = request.InstitutionView.SecuringPartyTypeId;  //Ensure that we cannot change the securd party type
            request.InstitutionView.ConvertToInstitution(institution); //Make sure that we do not change the membership info here
            auditTracker.Updated.Add(institution);

            //Create new membership service class
            MembershipServiceModel ms = new MembershipServiceModel(_institutionRepository);
            ms.AuditTracker = auditTracker; //Sets it audit tracker
            ms.EditClientInstitutionMembership(institution, request.InstitutionView.MembershipView.MembershipAccountTypeId, request.InstitutionView.MembershipView.AccountNumber.TrimNull(), request.InstitutionView.MembershipView.RepresentativeMembershipId,
                request.InstitutionView.MembershipView.MajorRoleIsSecuredPartyOrAgent,request.InstitutionView.MembershipView.isPayPointClient);


            #region auditing and committing
            //Audititng Processing
            AuditAction = AuditAction.ModifiedClientLegal ;
            AuditMessage = MembershipAuditMsgGenerator.InstitutionDetails(institution);
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion       

            //response.InstitutionView = GetClientInstitution(institution);
            response.InstitutionView = request.InstitutionView;
            response.InstitutionView.Id = institution.Id;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully edited institution";
            return response;
        }
        public ResponseBase ChangeClientInstitutionStatus(ChangeItemStatusRequest request)
        {
            ResponseBase response = new ResponseBase();

            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            Institution institution = _institutionRepository.FindBy(request.Id);    

            institution.IsActive = request.Activate;

            //Set up the AuditingTracker for auditing entities
            auditTracker.Updated.Add(institution);
            if (request.Activate == true)
                AuditAction = AuditAction.ActivatedClientLegal;
            else
                AuditAction = AuditAction.DeactivatedClientLegal;
            Audit audit = new Audit("Client Code:" + institution.Membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction);
            audit.Message = MembershipAuditMsgGenerator.InstitutionDetails(institution);
            _auditRepository.Add(audit);

            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            return new ResponseBase(request.Activate ==true ?  "Client activated": "Client deactivated", Infrastructure.Messaging.MessageType.Success);
        }
        public ResponseBase DeleteClientInstitution(DeleteItemRequest request)
        {

            ResponseBase response = new ResponseBase();

            if (!Constants.EnableDeletionOfInstitutions )
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "Cannot delete institution because deletion of institutions is turned off under configurations";
                return response;
            }

            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            Institution institution = _institutionRepository.FindBy(request.Id);
            List<User> users = _userRepository.GetDbSet().Where(s => s.InstitutionId == request.Id).ToList();
            List<InstitutionUnit> units = _institutionUnitRepository.GetDbSet().Where(s => s.InstitutionId == request.Id).ToList();
            foreach (var usr in users)
            {
                usr.IsDeleted = true;
            }
            foreach (var unit in units)
            {
                unit.IsDeleted = true;
            }

            //**Check to see if we are not reassigning stratus again
            //institution.Membership.IsDeleted = true;
            institution.IsDeleted = true;
            Audit audit = new Audit("Client Code:" + institution.Membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction.DeletedClientLegal);
            audit.Message = MembershipAuditMsgGenerator.InstitutionDetails(institution);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            response.MessageInfo.Message = "Successfully deleted Institution named " + institution .Name ;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
    }
}
