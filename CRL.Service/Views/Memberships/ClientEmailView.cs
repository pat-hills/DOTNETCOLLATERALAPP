using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;

namespace CRL.Service.Views.Memberships
{
    public class ClientEmailView
    {
        public int Id { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailFrom { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsRead { get; set; }
        public int NumRetries { get; set; }
        public bool? HasAttachment { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<FileUpload> EmailAttachments { get; set; }
    }
}
