using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.FS
{
    public class FinancialStatementComplexDuplicator
    {
        public FinancialStatement FS1 { get; set; }
        public FinancialStatement FS2 { get; set; }
        
    }

    public class FinancialStatementActivityComplexDuplicator
    {
        public FinancialStatementActivity  AS1 { get; set; }
        public FinancialStatementActivity AS2 { get; set; }

    }

    public class FinancialStatementComplexDuplicatorList
    {
        public List<FinancialStatementComplexDuplicator> _itemFS { get; set; }
        public List<FinancialStatementActivityComplexDuplicator> _itemsActivty { get; set; }
    

        public void ResolveAfterUpdateIds(int NewMembershipId)
        {
            foreach (var item in _itemFS)
            {
                item.FS2.MembershipId = NewMembershipId;
                if (item.FS1 .AfterUpdateFinancialStatementId != null)
                {
                    item.FS2.AfterUpdateFinancialStatementId = null;
                    item.FS2.AfterUpdateFinancialStatement = _itemFS.Where(s => s.FS1.Id == item.FS1.AfterUpdateFinancialStatementId).Single().FS2;

                }

            }
        }

        public void ResolveActivities(int NewMembershipId)
        {
            foreach (var item in _itemsActivty)
            {
                item.AS2.MembershipId = NewMembershipId;
                FinancialStatementComplexDuplicator activityFS = _itemFS.Where(s => s.FS1.Id == item.AS1.FinancialStatementId).Single();
                item.AS2.FinancialStatementId = 0;
                item.AS2.FinancialStatement = activityFS.FS2;

                if (item.AS1 is ActivityDischarge)
                {
                    var colls = activityFS .FS2 .Collaterals .Where (c=>c.DischargeActivityId == item.AS1.Id ).ToList ();
                    foreach (var coll in colls)
                    {
                    coll.DischargeActivityId = null;
                    coll.DischargeActivity = (ActivityDischarge)item.AS2;
                        //**Leave the FS Activity becuase we would not be assigning a si
                    }                
                  
                }             

                if (item.AS1 is ActivityUpdate )
                {
                    int? fid =  ((ActivityUpdate)item.AS1).PreviousFinancialStatementId ;
                    ((ActivityUpdate)item.AS2).PreviousFinancialStatementId = 0;
                    ((ActivityUpdate)item.AS2).PreviousFinancialStatement = _itemFS.Where(s => s.FS1.Id == fid).Single().FS2;

                }

            }

        }

      

       
    }
}
