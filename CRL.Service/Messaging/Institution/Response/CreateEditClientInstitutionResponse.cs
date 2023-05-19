using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Institution.Response
{
    public class CreateEditClientInstitutionResponse:ResponseBase
    {
        public InstitutionView InstitutionView { get; set; }
    }

    public class CreateEditInstitutionUnitResponse : ResponseBase
    {
        public InstitutionUnitView InstitutionUnitView { get; set; }
    }
}
