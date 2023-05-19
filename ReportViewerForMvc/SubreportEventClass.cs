using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;

namespace ReportViewerForMvc
{
    public abstract class SubreportEventClass
    {
        public abstract void SubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e);
       
    }
}
