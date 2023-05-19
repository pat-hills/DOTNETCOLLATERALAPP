using CRL.Model.FS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Infrastructure.Domain;
using CRL.Model.WorkflowEngine;
using CRL.Model.ModelViews;
using CRL.Model.FS.Enums;
using CRL.Model.Memberships;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class FinancialStatementActivity:AuditedEntityBaseModel<int>,IAggregateRoot
    {
        public FinancialStatementActivity()
        {
            this.Cases = new HashSet<WFCaseActivity>();
        }
        public string ActivityCode { get; set; }
        public int? PreviousFinancialStatementId { get; set; }
        public virtual FinancialStatement PreviousFinancialStatement { get; set; }
        
        //Relationships
        public FinancialStatementActivityCategory FinancialStatementActivityTypeId { get; set; }
        public virtual LKFinancialStatementActivityCategory FinancialStatementActivityType { get; set; }
        public int FinancialStatementId { get; set; }
        public virtual FinancialStatement FinancialStatement { get; set; }
        //public int PreviousFinancialStatementId { get; set; }
        //public virtual FinancialStatement PreviousFinancialStatement { get; set; }
        public virtual Membership Membership { get; set; }
        public int MembershipId { get; set; }
        public virtual InstitutionUnit InstitutionUnit { get; set; }
        public int? InstitutionUnitId { get; set; }   
        public int? VerificationAttachmentId { get; set; }
        public virtual FileUpload VerificationAttachment { get; set; }
        public short? isApprovedOrDenied { get; set; }
        public User ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RequestNo { get; set; }
        public string PreviousActivity { get; set; }
        public virtual ICollection<WFCaseActivity> Cases { get; set; }
        public bool NoChangeDetectedAfterPerformingChange { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }


        public FinancialStatementActivity  Duplicate()
        {
            FinancialStatementActivity activity = null;
            if (this is ActivityUpdate )
            {
                activity = ((ActivityUpdate)this).Duplicate();

            }
            else if (this is ActivityRenewal  )
            {
                activity =   ((ActivityRenewal)this).Duplicate();
            }
             else if (this is ActivityDischarge  )
            {
                activity = ((ActivityDischarge)this).Duplicate();
            }
            else if (this is ActivitySubordination )
            {
                activity = ((ActivitySubordination)this).Duplicate();
            }
            else if (this is ActivityAssignment )
            {
                activity = ((ActivityAssignment)this).Duplicate();
            }
            
            activity.ActivityCode = this.ActivityCode ;
            
            activity.CreatedBy = this.CreatedBy ;
            activity.CreatedOn = this.CreatedOn ;
            activity.FinancialStatementActivityTypeId = this.FinancialStatementActivityTypeId ;
            activity.FinancialStatementId = this.FinancialStatementId ;
            activity.IsActive = this.IsActive ;
            activity.isApprovedOrDenied  = this.isApprovedOrDenied ;
            activity.IsDeleted =this.IsDeleted ;
            activity.MembershipId = this.MembershipId ;
            activity.RequestNo = this.RequestNo ;
            activity.UpdatedBy = this.UpdatedBy ;
            activity.UpdatedOn = this.UpdatedOn ;
            activity.ApprovedDate = this.ApprovedDate;
            activity.VerificationAttachmentId = this.VerificationAttachmentId ;

            return activity;


        }
        
    }

    public partial  class LKFinancialStatementActivityCategory : EntityBase<FinancialStatementActivityCategory>, IAggregateRoot 
    {
        public string FinancialStatementActivityCategoryName { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


}
