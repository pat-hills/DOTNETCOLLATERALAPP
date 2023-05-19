using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Respositoy.EF.All.Common.Repository;

namespace CRL.Repository.EF.All.Repository
{
    public class SerialTrackerRepository : Repository<SerialTracker, SerialTrackerEnum>, ISerialTrackerRepository
    {
        public SerialTrackerRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "SerialTracker";
        }
    }
}
