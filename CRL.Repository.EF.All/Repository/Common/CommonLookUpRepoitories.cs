using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Common;
using CRL.Model.Common.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;

namespace CRL.Repository.EF.All.Repository.Common
{
    public class LKCountryRepository : Repository<LKCountry, int>, ILKCountryRepository
    {
        public LKCountryRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKCountry";
        }
    }

    public class LKNationalityRepository : Repository<LKNationality, int>, ILKNationalityRepository
    {
        public LKNationalityRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKNationality";
        }
    }

    public class LKCountyRepository : Repository<LKCounty, int>, ILKCountyRepository
    {
        public LKCountyRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKCounty";
        }
    }


    public class LKLGARepository : Repository<LKLGA, int>, ILKLGARepository
    {
        public LKLGARepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKLGAs";
        }
    }

    public class LKAuditActionRepository : Repository<LKAuditAction, int>, ILKAuditActionRepository 
    {
        public LKAuditActionRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKAuditAction";
        }
    }

    public class LKAuditCategoryRepository : Repository< LKAuditCategory, int>, ILKAuditCategoryRepository
    {
        public LKAuditCategoryRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKAuditCategory";
        }
    }


    public class LKRegistrationPrefixRepository : Repository<LKRegistrationPrefix, int>, ILKRegistrationPrefixRepository
    {
        public LKRegistrationPrefixRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "LKRegistrationPrefix";
        }
    }
}
