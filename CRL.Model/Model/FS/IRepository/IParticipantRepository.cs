using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.FS.IRepository
{
    public interface IParticipantRepository : IWriteRepository<Participant, int>
    {
    }
}
