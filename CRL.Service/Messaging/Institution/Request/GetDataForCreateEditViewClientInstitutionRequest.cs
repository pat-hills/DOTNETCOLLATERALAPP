using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.Institution.Request
{
    public class GetInstitutionRequest: RequestBase
    {
       
        public int? Id { get; set; }
        public EditMode EditMode { get; set; }
        public bool IgnoreLoadingInstitutionView { get; set; }
    }

    public class GetCreateInstitutionRequest : GetInstitutionRequest
    {

     
    }

    public class GetEditInstitutionRequest : GetInstitutionRequest
    {

    }

    public class GetViewInstitutionRequest : GetInstitutionRequest
    {

    }

    public class GetInstitutionUnitRequest : RequestBase
    {

        public int? Id { get; set; }
        public EditMode EditMode { get; set; }
        public bool IgnoreLoadingInstitutionUnitView { get; set; }
        
        
    }
}
