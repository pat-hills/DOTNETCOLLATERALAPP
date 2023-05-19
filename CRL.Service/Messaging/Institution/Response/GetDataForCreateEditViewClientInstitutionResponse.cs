using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;


using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Institution.Response
{
  
    public class GetInstitutionResponse:ResponseBase
    {
        //Get the membership type        
        public ICollection<LookUpView> SecuringPartyTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
      
        //
        public ICollection<LookUpView> LGAs { get; set; }
        public InstitutionView InstitutionView { get; set; }
    }

    public class GetCreateInstitutionResponse : GetInstitutionResponse
    {
       
    }

    public class GetEditInstitutionResponse : GetInstitutionResponse
    {

    }

    public class GetViewInstitutionResponse : GetInstitutionResponse
    {

    }
    public class GetInstitutionUnitResponse : ResponseBase
    {
        //Get the membership type        
       public InstitutionUnitView InstitutionUnitView { get; set; }
    }
}
