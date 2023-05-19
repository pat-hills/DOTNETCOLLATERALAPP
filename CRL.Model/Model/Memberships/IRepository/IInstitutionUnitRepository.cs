using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;

namespace CRL.Model.Memberships.IRepository
{
    public interface IInstitutionUnitRepository : IWriteRepository<InstitutionUnit, int>
    {
        

              InstitutionUnit GetUnitDetailById(int id);    
              IQueryable<InstitutionUnit> GetDbSetComplete();

              ViewInstitutionUnitsResponse UnitsGridView(ViewInstitutionUnitsRequest request);
    }
}
