using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;

namespace CRL.Model.Common
{
    [Serializable ]
    public class AddressInfo
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
   


    }
    [Serializable ]
    public class PersonIdentificationInfo
    {
      
        public string FirstName { get; set; }       
        public string MiddleName { get; set; }    
        public string Surname { get; set; }       
        public string CardNo { get; set; }
        public string CardNo2 { get; set; } 
        public string OtherDocumentDescription { get; set; }
        
        


    }
}
