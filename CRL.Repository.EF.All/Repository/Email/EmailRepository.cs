using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Notification.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;

namespace CRL.Repository.EF.All.Repository.Email
{
 
    public class EmailRepository : Repository<Model.Notification .Email  , int>, IEmailRepository
    {
        public EmailRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Email";
        }

        public ICollection<Model.Notification.Email> GetEmailsByUserId(int userId)
        {
            var query = ctx.Emails.Where(s => s.EmailUserAssignments.Any(c => c.UserId == userId && c.IsRead == false)).OrderByDescending(s=>s.Id).Take(5);
            return query.ToList();
        }
    }
}
