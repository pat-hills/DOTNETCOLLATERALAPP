using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;

using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Workflow;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
 
    public class GetDataForFSByEditModeResponse : ResponseBase
    {         
        public ICollection<LookUpView> CollateralTypes { get; set; }      
        public ICollection<LookUpView> Currencies { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
        public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
        public ICollection<LookUpView> IdentificationCardTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
        public ICollection<LookUpView> DebtorTypes { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> LGAs { get; set; }
        public ICollection<LookUpView> CollateralSubTypes { get; set; }
        public ICollection<LookUpView> BusinessPrefixes { get; set; }
        public FSView FSView { get; set; }
        public  IEnumerable<FSActivityGridView> ActivityGridView { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public bool HasDraftLoad { get; set; }
        public bool HasDownload { get; set; }
        public string DownloadFileName { get; set; }
        public bool isPayable { get; set; }
        public decimal? Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public bool CreateModeIsWorkflowOn { get; set; }
         public bool AmountIsBasedOnLoanAmount { get; set; }
         public bool IsNotOwnerOfBatch { get; set; }
    
    
      
    }
}
