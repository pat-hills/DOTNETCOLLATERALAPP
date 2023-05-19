using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Memberships;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Institution.Request
{
    public class CreateEditClientInstitutionRequest:RequestBase
    {
        public InstitutionView InstitutionView { get; set; }
        
    }

    public class CreateEditInstitutionUnitRequest : RequestBase
    {
        public InstitutionUnitView InstitutionUnitView { get; set; }

    }
}
