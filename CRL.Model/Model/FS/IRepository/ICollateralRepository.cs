﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.FS.IRepository
{
    public interface ICollateralRepository : IWriteRepository<Collateral, int>
    {
        IQueryable<Collateral> GetDbSetWithCategoryNames();
    }
}
