using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;

namespace CRL.Infrastructure.Messaging
{
    public class RequestBase
    {
        public RequestBase() { }
        
        public int Id { get; set; }
        public SecurityUser SecurityUser { get; set; }
        public string RequestUrl { get; set; }
        public string UserIP { get; set; }
        public string UniqueGuidForm { get; set; }
        public RequestMode RequestMode { get; set; }

         public void CloneTo(RequestBase request)
        {
            request.Id  = Id;
            request.SecurityUser  = this.SecurityUser ;
            request.RequestUrl  = this.RequestUrl ;
            request.UserIP  = this.UserIP ;
            request.UniqueGuidForm = this.UniqueGuidForm;
            request.RequestMode = this.RequestMode;

        }
    }
}
