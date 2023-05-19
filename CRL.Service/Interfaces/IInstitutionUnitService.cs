using System;
using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Institution.Response;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
namespace  CRL.Service.Interfaces{
   public interface IInstitutionUnitService
    {
        ResponseBase ChangeInstitutionUnitStatus(CRL.Service.Messaging.Common.Request.ChangeItemStatusRequest request);
        CreateEditInstitutionUnitResponse CreateInstitutionUnit(CreateEditInstitutionUnitRequest request);
        ResponseBase DeleteInstitutionUnit(CRL.Service.Messaging.Common.Request.DeleteItemRequest request);
        CRL.Service.Messaging.Institution.Response.CreateEditInstitutionUnitResponse EditInstitutionUnit(CRL.Service.Messaging.Institution.Request.CreateEditInstitutionUnitRequest request);
        GetInstitutionUnitResponse GetCreate(GetInstitutionUnitRequest request);
        GetInstitutionUnitResponse GetEdit(GetInstitutionUnitRequest request);
        GetInstitutionUnitResponse GetView(GetInstitutionUnitRequest request);
        ViewInstitutionUnitsResponse ViewInstitutionUnits(ViewInstitutionUnitsRequest request);
    }
}
