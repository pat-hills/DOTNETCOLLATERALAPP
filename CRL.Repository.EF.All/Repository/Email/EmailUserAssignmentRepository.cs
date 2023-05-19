using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Repository.EF.All.Repository.Email
{
    public class EmailUserAssignmentRepository : Repository<EmailUserAssignment, int>, IEmailUserAssignmentRepository
    {
        public EmailUserAssignmentRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "EmailUserAssignment";
        }

        
    }
   
}
