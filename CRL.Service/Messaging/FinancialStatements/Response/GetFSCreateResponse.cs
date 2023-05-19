using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetFSViewResponse : ResponseBase
    {
        public FSView FSView { get; set; }
        public IEnumerable<FSActivityGridView> FSActivityGridView { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
    }
    public class GetFSLookUpsResponse : ResponseBase
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
        public ICollection<LookUpView> Countydetailed { get; set; }
        public ICollection<LookUpView> CollateralSubTypes { get; set; }
        public ICollection<LookUpView> LGAs { get; set; }
        public ICollection<LKLGAView> LGAdetailed { get; set; }
        public ICollection<CollateralSubTypeView> CollateralSubTypeDetailed { get; set; }

        public ICollection<LookUpView> BusinessPrefixes { get; set; }
    }
    public class GetFSEditResponse : GetFSLookUpsResponse
    {
        public FSView FSView { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        

    }

    public class GetEditUpdateResponse : GetFSEditResponse
    {
        public int OriginalFinancingStatementId { get; set; }
    }

    public class PaymentInfo
    {
        public bool isPayable { get; set; }
        public decimal? Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public bool AmountIsBasedOnLoanAmount { get; set; }
    }
    public class GetFSCreateResponse : GetFSLookUpsResponse
    {


        public bool? CreateAsRegistrant { get; set; }
        //public FinancialStatementTransactionCategory FSTransactionCategory { get; set; }
        public FSView FSView { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public bool IsNotOwnerOfBatch { get; set; }




    }
}
