using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Notification.IRepository
{
    public interface IEmailRepository : IWriteRepository<Email, int>
    {
        ICollection<Email> GetEmailsByUserId(int userId);
    }

    public interface IEmailUserAssignmentRepository : IWriteRepository<EmailUserAssignment, int>
    {
        
    }
}
