using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.Memberships.Request
{
    public enum RegistrationType { Individual = 1, Institution = 2 }
    public class GetCreateRequest:RequestBase
    {
         
        public int? Id { get; set; }
        public EditMode EditMode { get; set; }
        public bool IgnoreLoadingInstitutionView { get; set; }
          public RegistrationType CreateRegistrationType { get; set; }
          public string ConfirmCode { get; set; }
    
    }
}
