using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Messaging
{
   

    public class ViewInstitutionUnitsRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// This is the only parameter that is mandatory since without it we will be loading all institutionunit for every institution
        /// </summary>
        public int InstitutionId { get; set; } 
      
      

    }
}
