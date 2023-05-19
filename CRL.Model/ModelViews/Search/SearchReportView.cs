using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Views.Search
{
    public class SearchReportView : FileUploadView
    {
        public string RegistrationNo;
        public int SearchId;
        public DateTime DateGenerated { get; set; }
    }
}
