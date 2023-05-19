using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Administration
{
  public  class EmailView
    {
         public int Id { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailFrom { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsActive { get; set; }       
        public int NumRetries { get; set; }
        public DateTime? LastRetryDate { get; set; }
        public bool? FailedMail { get; set; }
        public bool? HasAttachment { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
