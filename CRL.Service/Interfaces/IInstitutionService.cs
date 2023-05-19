using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

using CRL.Service.Messaging.Common.Request;
using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Institution.Response;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.Interfaces
{
    public interface IInstitutionService
    {
        GetCreateInstitutionResponse GetCreate(GetCreateInstitutionRequest request);
        GetEditInstitutionResponse GetEdit(GetEditInstitutionRequest request);
        GetViewInstitutionResponse GetView(GetViewInstitutionRequest request);
        ViewClientInstitutionsResponse ViewClientInstitutions(ViewClientInstitutionsRequest request);
        CreateEditClientInstitutionResponse CreateClientInstitution(CreateEditClientInstitutionRequest request);
        CreateEditClientInstitutionResponse EditClientInstitution(CreateEditClientInstitutionRequest request);
        ResponseBase ChangeClientInstitutionStatus(ChangeItemStatusRequest request);
        ResponseBase DeleteClientInstitution(DeleteItemRequest request);

    }
}
