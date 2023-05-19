using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Reporting;

using CRL.Service.Views.Report;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Reporting.Response
{
    public class GetReportByIdResponse:ResponseBase
    {
        public Report Report { get; set; }
    }

    public class ViewReportsResponse : ResponseBase
    {
        public List<ReportView> ReportsView { get; set; }
    }
}
