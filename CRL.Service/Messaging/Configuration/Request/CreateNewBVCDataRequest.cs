using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.Configuration.Request
{
   public  class CreateNewBVCDataRequest:RequestBase
    {
        
      public CreateNewBVCDataRequest()
      
      {
          BankVerificationCodesView = new List<BankVerificationCodeView>();


      }
      public ICollection<BankVerificationCodeView> BankVerificationCodesView { get; set; }
    }

    

   
    
}
