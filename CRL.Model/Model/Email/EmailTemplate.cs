using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.Notification
{
    public class EmailTemplate : AuditedEntityBaseModel<int>, IAggregateRoot
    {
     
        public string EmailAction { get; set; }
        [MaxLength(255)]
        public string EmailSubject { get; set; }
           [MaxLength(10000)]
        public string EmailBodyHTML { get; set; }

    
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
