using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;

namespace CRL.Repository.EF.All.Repository.Email
{


    public class EmailTemplateRepository : Repository<EmailTemplate, int>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "EmailTemplate";
        }
    }
}
