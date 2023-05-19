using CRL.Model.FS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Common
{
    public interface IIndividual
    {   
         string Title { get; set; }
         string FirstName { get; set; }
         string MiddleName { get; set; }
         string Surname { get; set; }
         string Phone { get; set; }        
         string Gender { get; set; }
         string Email { get; set; }
         string Fax { get; set; }
         string Address { get; set; }
         System.Nullable<DateTime> DOB { get; set; }
         System.Nullable<int> CountryId { get; set; }
         System.Nullable<int> NationalityId { get; set; }       
        
    }

    public interface IDetailIndividual : IIndividual
    {

        ICollection<PersonIdentification> IdentificationCard { get; set; }
         string FamilyBook { get; set; }
       
    }

   
}
