using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.BusinessServices
{
    public class FSBatch
    {
        public FSBatch()
        {

            FinancingStatements = new List<FSView>();
            NoOfFSInBatch = FinancingStatements.Count();
            FSBatchCreationDate = DateTime.Now;
        }

        public string NumberOFFS { get; set; }
        public string BatchName { get; set; }

        DateTime _fsBatchCreationDate;
        public DateTime FSBatchCreationDate
        {
            get { return _fsBatchCreationDate; }
            set
            {
                _fsBatchCreationDate = value;
            }
        }

        int _noOfFsInBatch;
        public int NoOfFSInBatch
        {
            get { return _noOfFsInBatch; }

            set
            {
                _noOfFsInBatch = value;

            }

        }

        public List<FSView> FinancingStatements { get; set; }
    }
}
