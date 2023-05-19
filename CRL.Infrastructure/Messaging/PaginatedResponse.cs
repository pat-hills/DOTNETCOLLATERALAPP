using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Messaging
{
    public class PaginatedRequest:RequestBase
    {
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class ReportRequestBase : PaginatedRequest
    {

        public bool NotPaginateRecords { get; set; }
    }
}
