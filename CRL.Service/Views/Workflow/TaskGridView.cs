using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Workflow
{
    public class TaskGridView
    {
        public int Id { get; set; }
        public string CaseTitle { get; set; }
        public string Placename { get; set; }
        public string CaseType { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
