using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.FinancialStatement
{
    public class FSBatchView
    {
        public string Name { get; set; }
        public bool IsSettled { get; set; }
        public int NumberOfFS { get; set; }
        public int NumberOfUploadedFS { get; set; }
        public int RemainingFSToBeUploaded { get; set; }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
