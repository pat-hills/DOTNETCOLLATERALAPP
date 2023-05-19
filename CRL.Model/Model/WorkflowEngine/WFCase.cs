using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Enums;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Model.Memberships;

namespace CRL.Model.WorkflowEngine
{
    public partial class WFCase : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WFCase()
        {
            Tokens = new HashSet<WFToken >();
            WorkItems = new HashSet<WFWorkItem>();
            CaseMails= new HashSet<WFCaseMail>();
            this.CaseStatus = "OP";
            this.CaseStartDate = DateTime.Now;
            this.LockItem = false;
        }
        
        public string CaseContext { get; set; }
        public string CaseTitle { get; set; }
        public string CaseStatus { get; set; }
        public WorkflowRequestType CaseType { get; set; }
        public WFTaskType  TaskType { get; set; }
        public bool LockItem { get; set; }
        public DateTime  CaseStartDate { get; set; }
        public DateTime? CaseEndDate { get; set; }
        public int? LimitedToOtherMembershipId { get; set; }
        public Membership  LimitedToOtherMembership { get; set; }
        public int? LimitedToOtherUnitId { get; set; }
        public InstitutionUnit LimitedToOtherInstitutionUnit { get; set; }
        public int WorkflowId { get; set; }
        public virtual WFWorkflow Workflow { get; set; }
        public virtual ICollection<WFToken> Tokens { get; set; }
        public virtual ICollection<WFWorkItem> WorkItems { get; set; }
        public virtual ICollection<WFCaseMail> CaseMails { get; set; }
      
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }

        public void SetContextNo(string ContextNo)
        {
            this.CaseContext = ContextNo;
            foreach (var token in this.Tokens)
            {
                token.TokenContext = ContextNo;
            }
            foreach (var workitem in this.WorkItems)
            {
                workitem.Context = ContextNo;
            }
        }

    }

    public partial class WFCaseMail : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string Email { get; set; }
        public int CaseId { get; set; }
        public WFCase Case { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }




    public partial class WFCaseFS : WFCase
    {
        public int FinancialStatementId { get; set; }
        public virtual FinancialStatement FinancialStatement { get; set; }
    }

    public partial class WFCaseActivity : WFCase
    {
        public int FinancialStatementActivityId { get; set; }
        public virtual FinancialStatementActivity FinancialStatementActivity { get; set; }
    }

    public partial class WFCasePostpaidSetup : WFCase
    {
        public int MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public string AccountNumber {get;set;}
        public int? RepresentativeMembershipId{get;set;}
        public MembershipAccountCategory MembershipAccountTypeId { get; set; }
            
    }

    public partial class WFCaseMembershipRegistration : WFCase
    {
        public int MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
    }

    public partial class WFCasePaypointUsersAssignment : WFCase
    {
        public WFCasePaypointUsersAssignment()
        {
            Users = new HashSet<User>();
        }
        public virtual  ICollection<User> Users { get; set; }
    }
}
