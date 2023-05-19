﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Reporting.IRepository
{
  
    public interface IReportRepository : IWriteRepository<Report, int>
    {
      
    }
}
