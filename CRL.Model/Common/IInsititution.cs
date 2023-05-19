using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Common
{
    public interface IInsititution
    {
         string Name{get;set;}
         string Description { get; set; }
         string Email { get; set; }
         string Fax { get; set; }
         string Address { get; set; }


    }

    public interface IDetailInstitution:IInsititution
    {
       
         int InstitutionTypeId { get; set; }
         string CompanyNo { get; set; }        
         string TaxNo { get; set; }
         Nullable<DateTime> IncorporationDate { get; set; }
         int CompanySize { get; set; }
         float LoanAmount { get; set; }
         string GenderComposition { get; set; }
    }
}
