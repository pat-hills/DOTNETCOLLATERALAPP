using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS.Enums;
using CRL.Model.Infrastructure;

namespace CRL.Model
{
    public partial class ServiceRequest : EntityBase<int>, IAggregateRoot
    {
        private IServiceRequestRepository _serviceRequestRepository;
        public ServiceRequest()
        {

        }
        public ServiceRequest(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }




        public string RequestNo { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }



    }

    public partial class Audit : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public Audit()
        {

        }
        public Audit(string Msg, string RequestUrl, string UserIp, AuditAction AuditAction)
        {
            this.Message = Msg;
            
            this.RequestUrl = RequestUrl;
            this.MachineName = UserIp;
            this.AuditActionId = AuditAction;

        }
        [MaxLength(360)]
        public string Message { get; set; }
        [MaxLength(100)]
        public string RequestUrl { get; set; }
        [MaxLength(50)]
        public string MachineName { get; set; }
        public virtual LKAuditAction AuditAction { get; set; }
        public AuditAction   AuditActionId { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class LKAuditAction : EntityBase<AuditAction>, IAggregateRoot
    {
         [MaxLength(100)]
        public string Name { get; set; }

        public virtual LKAuditCategory AuditType { get; set; }
        public AuditCategory AuditTypeId { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class LKAuditCategory : EntityBase<AuditCategory>, IAggregateRoot
    {
         [MaxLength(50)]
        public string Name { get; set; }




        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

  
}
