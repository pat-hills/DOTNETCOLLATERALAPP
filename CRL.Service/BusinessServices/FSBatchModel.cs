using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.BusinessServices
{

    public enum BatchFIleStatus { SavedOnly = 1, Generated = 2 }
    [Serializable()]
    public class FSBatchModel// : ISerializable
    {

        public FSBatchModel()
        {

            FinancingStatements = new List<FSView>();
            NoOfFSInBatch = FinancingStatements.Count();
            FSBatchCreationDate = DateTime.Now;
            IdTracker = 0;

        }

        public ICollection<FSView> FinancingStatements { get; set; }

        string _fsBatchName { get; set; }

        public int IdTracker { get; set; }
        public string FSBatchName
        {
            get { return _fsBatchName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException();
                _fsBatchName = value;
            }

        }

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


        public int ApplicationVersionNumber { get; set; }

        public BatchFIleStatus FileStatus { get; set; }




        public FSBatchModel(SerializationInfo info, StreamingContext ctxt)
        {
            //Get the values from info and assign them to the appropriate properties
            FinancingStatements = (ICollection<FSView>)info.GetValue("FinancingStatements", typeof(ICollection<FSView>));
            FSBatchName = (String)info.GetValue("FSBatchName", typeof(string));
            FSBatchCreationDate = (DateTime)info.GetValue("FSBatchCreationDate", typeof(DateTime));
            NoOfFSInBatch = (int)info.GetValue("NoOfFSInBatch", typeof(int));
            IdTracker = (int)info.GetValue("IdTracker", typeof(int));
            ApplicationVersionNumber = (int)info.GetValue("ApplicationVersionNumber", typeof(int));
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            //You can use any custom name for your name-value pair. But make sure you
            // read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
            // then you should read the same with "EmployeeId"
            info.AddValue("FinancingStatements", FinancingStatements);
            info.AddValue("FSBatchName", FSBatchName);
            info.AddValue("FSBatchCreationDate", FSBatchCreationDate);
            info.AddValue("NoOfFSInBatch", NoOfFSInBatch);
            info.AddValue("IdTracker", IdTracker);
            info.AddValue("ApplicationVersionNumber", ApplicationVersionNumber);
        }


    }
}
