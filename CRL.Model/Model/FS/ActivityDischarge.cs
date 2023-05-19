using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class ActivityDischarge:FinancialStatementActivity
    {
        public ActivityDischarge()
        {
            Collaterals=  new HashSet<Collateral>();
        }
        public int DischargeType { get; set; }
        public bool PerformedByRegistrar { get; set; }
        
        //Get list of partial discharge collaterals
        public virtual ICollection<Collateral> Collaterals { get; set; }

        //Discharge, Partial Discharge
        //SubmitForDischarge,

        public new ActivityDischarge Duplicate()
        {
            ActivityDischarge activity = new ActivityDischarge();
            activity.DischargeType  = this.DischargeType ;
            
            
            return activity;
        }

        public void SubmitDischarge(int[] CollateralsToPartiallyDischargeIds, AuditingTracker auditTracker = null)
        {
            if (CollateralsToPartiallyDischargeIds.Length == 0)
            {
                this.FinancialStatement.DischargeActivity = this;

                foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => s.IsDeleted == false && s.IsActive == true && s.IsDischarged == false))
                {
                    collateral.DischargeActivity = this;
                }


            }

            else
            {

                foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => CollateralsToPartiallyDischargeIds.Contains(s.Id)))
                {
                    collateral.DischargeActivity = this;
                }

            }


        }

        public void CreateDischarge(int[] CollateralsToPartiallyDischargeIds, AuditingTracker auditTracker = null)
        {
            if (CollateralsToPartiallyDischargeIds.Length == 0)
            {
                this.FinancialStatement.DischargeActivity = this;
                this.FinancialStatement.IsDischarged  = true;

                foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => s.IsDeleted == false && s.IsActive == true && s.IsDischarged == false))
                {
                    collateral.DischargeActivity = this;
                    collateral.IsDischarged = true;
                    if (auditTracker != null)
                    {
                        auditTracker.Updated.Add(collateral);
                    }
                }


            }

            else
            {

                foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => CollateralsToPartiallyDischargeIds.Contains(s.Id)))
                {
                    collateral.DischargeActivity = this;
                    collateral.IsDischarged = true;
                    if (auditTracker != null)
                    {
                        auditTracker.Updated.Add(collateral);
                    }
                }

            }

            //*Set the entire financial statement to discharge
        }
        public void ApproveDischarge(AuditingTracker auditTracker = null)
        {
            if (this.FinancialStatement.DischargeActivityId != null)
            {
                this.FinancialStatement.IsDischarged = true;
            }
            foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => s.IsDeleted == false && s.IsActive == true && s.DischargeActivity !=null && s.IsDischarged ==false))
            {
                collateral.IsDischarged = true;
                collateral.DischargeActivity = this;
                if (auditTracker != null)
                {
                    auditTracker.Updated.Add(collateral);
                }
            }

            //*Set the entire financial statement to discharge
        }

        public void DenySubmitDischarge(AuditingTracker auditTracker = null)
        {
            if (this.FinancialStatement.DischargeActivityId != null)
            {
                this.FinancialStatement.DischargeActivity =null;
                this.FinancialStatement.DischargeActivityId = null;
            }

            foreach (var collateral in this.FinancialStatement.Collaterals.Where(s => s.IsDeleted == false && s.IsActive == true && s.DischargeActivity != null))
            {
                
                collateral.DischargeActivity = null;
                collateral.DischargeActivityId = null;
                auditTracker.Updated.Add(collateral);
            }

            //*Set the entire financial statement to discharge
        }
     


    }
}
