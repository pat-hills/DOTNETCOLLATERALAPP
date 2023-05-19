using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Common.IRepository
{
    public interface ILKCountyRepository : IWriteRepository<LKCounty, int>
    {
    }
    public interface ILKCountryRepository : IWriteRepository<LKCountry, int>
    {
    }
    public interface ILKNationalityRepository : IWriteRepository<LKNationality, int>
    {
    }

    public interface ILKLGARepository : IWriteRepository<LKLGA, int>
    { 
    }


    public interface ILKRegistrationPrefixRepository : IWriteRepository<LKRegistrationPrefix, int>
    {
    }
}
