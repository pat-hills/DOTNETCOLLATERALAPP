using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;

using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Institution.Response;

using CRL.Service.Mappings .Membership;
using CRL.Service.Views.Memberships;
using CRL.Service.QueryGenerator;
using CRL.Infrastructure.Domain;
using CRL.Service.Messaging.Common.Request;

using CRL.Service.Interfaces;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViewMappers;
using CRL.Service.Common;
using CRL.Model.Messaging;
using CRL.Model;
using CRL.Model.FS.Enums;
using CRL.Service.BusinessService;
using CRL.Model.Memberships;
using CRL.Infrastructure.Configuration;

namespace CRL.Service.Implementations
{
    public class InstitutionUnitService : ServiceBase,IInstitutionUnitService
    {
        public GetInstitutionUnitResponse GetCreate(GetInstitutionUnitRequest request)
        {
            GetInstitutionUnitResponse response = new GetInstitutionUnitResponse();        
            
            if (!request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            
            return response;
        }

        public GetInstitutionUnitResponse GetEdit(GetInstitutionUnitRequest request)
        {
            GetInstitutionUnitResponse response = new GetInstitutionUnitResponse();

            if (!request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            InstitutionUnit institutionUnit = _institutionUnitRepository.GetDbSet().Where(s=>s.IsDeleted ==false && s.Id == request.Id).Single();
            response.InstitutionUnitView = institutionUnit.ConvertToInstitutionUnitView();


            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public GetInstitutionUnitResponse GetView(GetInstitutionUnitRequest request)
        {
            GetInstitutionUnitResponse response = new GetInstitutionUnitResponse();
            
            //BUSRL:Ensure that only administrators can see units not from their institutions
            //**01_Change
            if (!request.SecurityUser.IsOwnerAdminRegistrySupport() && !request .SecurityUser .IsClientAdministrator () )
            {
                
                if (request.SecurityUser.InstitutionUnitId != request.Id)
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
                
            }

         


            InstitutionUnit institutionUnit = _institutionUnitRepository.GetUnitDetailById((int)request.Id);

            //BUSRL:Ensure that only owners can see units belonging to other institutions
            //**01_Change
            if (!request.SecurityUser.IsOwnerAdminRegistrySupport())
            {

                if (request.SecurityUser.InstitutionId != institutionUnit.InstitutionId)
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }
            response.InstitutionUnitView = institutionUnit.ConvertToInstitutionUnitView();
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

       



       

      


        public ViewInstitutionUnitsResponse ViewInstitutionUnits(ViewInstitutionUnitsRequest request)
        {
            if (request.InstitutionId < 1)
            {
                request.InstitutionId =(int)request.SecurityUser.InstitutionId;
            }

            ViewInstitutionUnitsResponse response = new ViewInstitutionUnitsResponse();

            if (!request.SecurityUser.IsOwnerAdminRegistrySupport ())
            {
                if (!request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId != request.InstitutionId)
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }

            response = _institutionUnitRepository.UnitsGridView (request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;   

            
          
          
        }


        //========================================================================================

        public CreateEditInstitutionUnitResponse CreateInstitutionUnit(CreateEditInstitutionUnitRequest request)
        {
            CreateEditInstitutionUnitResponse response = new CreateEditInstitutionUnitResponse();

            if (!request.SecurityUser.IsOwnerAdministrator ())
            {
                if (!(request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId == request.InstitutionUnitView.InstitutionId))
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }

            if (_institutionUnitRepository.GetDbSet().Where(s => s.Name.Trim().ToLower() == request.InstitutionUnitView.Name.Trim().ToLower() && s.InstitutionId == request.InstitutionUnitView.InstitutionId && s.IsDeleted == false).Count() > 0 )
            { 
               response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                response.MessageInfo.Message = "The institution Unit name is already being used within this institution";//**Add the validation error here
                return response;
            }
           
            //Setup the InstitutionUnit
            InstitutionUnit InstitutionUnit = request.InstitutionUnitView.ConvertToInstitutionUnit();
            _institutionUnitRepository.Add(InstitutionUnit);
            auditTracker.Created.Add(InstitutionUnit); //Set the InstitutionUnit as audited



            #region auditing and committing
            //Audititng Processing
            AuditAction = AuditAction.CreatedUnit;
            AuditMessage = MembershipAuditMsgGenerator.InstitutionUnitDetails(InstitutionUnit);
            var responseAfterCommit = this.AuditCommit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion      

            response.InstitutionUnitView = request.InstitutionUnitView;
            response.InstitutionUnitView.Id = InstitutionUnit.Id;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully added new unit";
            return response;
        }

        public CreateEditInstitutionUnitResponse EditInstitutionUnit(CreateEditInstitutionUnitRequest request)
        {
            CreateEditInstitutionUnitResponse response = new CreateEditInstitutionUnitResponse();

            if (!request.SecurityUser.IsOwnerAdministrator())
            {
                if (!(request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId == request.InstitutionUnitView.InstitutionId))
                {
                    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                    return response;
                }
            }
            InstitutionUnit institutionUnit;

        
                institutionUnit = _institutionUnitRepository.GetDbSet().Where(item => item.Id == request.InstitutionUnitView.Id).Single ();


                if (institutionUnit.Name.ToLower().Trim() != request.InstitutionUnitView.Name.ToLower().Trim())
                {
                    if (_institutionUnitRepository.GetDbSet().Where(s => s.Name.Trim().ToLower() == request.InstitutionUnitView.Name.Trim().ToLower() && s.InstitutionId == request.InstitutionUnitView.InstitutionId && s.Id != institutionUnit.Id && s.IsDeleted == false).Count() > 0)
                    {
                        response.MessageInfo = new Infrastructure.Messaging.MessageInfo();
                        response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                        response.MessageInfo.Message = "The institution Unit name is already being used within this institution";//**Add the validation error here
                        return response;
                    }
                }


            request.InstitutionUnitView.ConvertToInstitutionUnit(institutionUnit);
            auditTracker.Updated.Add(institutionUnit);
            #region auditing and committing
            //Audititng Processing
            AuditAction = AuditAction.ModifiedUnit ;
            AuditMessage = MembershipAuditMsgGenerator.InstitutionUnitDetails(institutionUnit);
            var responseAfterCommit = this.AuditCommit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion 


            response.InstitutionUnitView = request.InstitutionUnitView;
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully edited unit";
            return response;
        }

        
        public ResponseBase ChangeInstitutionUnitStatus(ChangeItemStatusRequest request)
        {
            ResponseBase response = new ResponseBase();
            InstitutionUnit institutionUnit = _institutionUnitRepository.FindBy(request.Id);

            if (!request.SecurityUser.IsOwnerAdministrator())
            {
                if (!request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId != institutionUnit.InstitutionId)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                    return response;
                }
            }



            institutionUnit.IsActive = request.Activate;

            //Set up the AuditingTracker for auditing entities
            auditTracker.Updated.Add(institutionUnit);
            if (request.Activate == true)
                AuditAction = Model.FS.Enums.AuditAction.ActivatedUnit ;
            else
                AuditAction = Model.FS.Enums.AuditAction.DeactivatedUnit ;
            Audit audit = new Audit("Institution Unit ID:" + institutionUnit.Id + " Institution Unit Name:" + institutionUnit.Name + " Institution:" + institutionUnit.Institution.Name, 
                request.RequestUrl, request.UserIP, AuditAction);
            
            _auditRepository.Add(audit);

            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            return new ResponseBase(request.Activate == true ? "Unit activated" : "Unit deactivated", Infrastructure.Messaging.MessageType.Success);
        }
        public ResponseBase DeleteInstitutionUnit(DeleteItemRequest request)
        {

            ResponseBase response = new ResponseBase();

            if (!Constants.EnableDeletionOfInstitutionUnits )
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "Cannot delete institution unit because deletion of institution units is turned off under configurations";
                return response;
            }


              InstitutionUnit institutionUnit = _institutionUnitRepository.FindBy(request.Id);

            if (!request.SecurityUser.IsOwnerAdministrator ())
            {
                if (!request.SecurityUser.IsClientAdministrator() && request.SecurityUser.InstitutionId != institutionUnit.InstitutionId)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                    return response;
                }
            }

          

            //**Check to see if we are not reassigning stratus again

            institutionUnit.IsDeleted = true;
            List<User> users = _userRepository.GetDbSet().Where(s => s.InstitutionUnitId  == request.Id).ToList();
          
            foreach (var usr in users)
            {
                usr.IsDeleted = true;
            }
        
            Audit audit = new Audit("Institution Unit ID:" + institutionUnit.Id + " Institution Unit Name:" + institutionUnit .Name + " Institution:" + institutionUnit.Institution .Name 
            , request.RequestUrl, request.UserIP, Model.FS.Enums.AuditAction.DeletedUnit );
            
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            response.MessageInfo.Message = "Unit deleted";
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
    }

}
