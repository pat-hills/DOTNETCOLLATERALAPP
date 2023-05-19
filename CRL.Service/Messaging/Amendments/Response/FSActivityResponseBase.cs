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
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class FSActivityResponseBase
    {
        public DateTime DateOfActivity { get; set; }
        public string ActivityCode { get; set; }

        public ParticipantSummaryView[] Participants { get; set; }
        public FSSummaryView FinancialStatement { get; set; }
        public CollateralSummaryView[] Collaterals { get; set; }

        public ParticipantSummaryView[] NextParticipants { get; set; }
        public FSSummaryView NextFinancialStatement { get; set; }
        public CollateralSummaryView[] NextCollaterals { get; set; }

        public ParticipantSummaryView[] PreviousParticipants { get; set; }
        public FSSummaryView PreviousFinancialStatement { get; set; }
        public CollateralSummaryView[] PreviousCollaterals { get; set; }
    }

    public class GetDataForAmendResponse : GetFSLookUpsResponse
    {
        public FSView FSView { get; set; }
        public CollateralView[] CollateralsToDischarge { get; set; }

        public ICollection<LookUpView> LegalEntityClients { get; set; }
        public ICollection<LookUpView> IndividualClients { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
        public ICollection<LookUpView> PersonIdentificationCards { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public SubordinatingPartyView SubordinatingPartyView { get; set; }
        public InstitutionView InstitutionView { get; set; }
        public bool isPayable { get; set; }
        public decimal Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public byte[] FSRowVersion { get; set; }
        public bool CreateModeIsWorkflowOn
        {
            get;
            set;


        }

    }
    public class GetDataForAmendViewListResponse : ResponseBase
    {
        public ICollection<LookUpView> FinancialStatementActivityType { get; set; }
    }

    public class GetDataForAmendViewResponse : ResponseBase
    {
        public FSActivityDetailSummaryView FSActivityDetailSummaryView { get; set; }
        public FSView FSView { get; set; }
        public FSView BeforeUpdateFSView { get; set; }
        public FSView AfterUpdateFSView { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
    }

    public class GetCreateUpdateResponse : GetFSLookUpsResponse
    {
        public FSView FSView { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public int? EditOfOriginalFinancialStatementID { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public bool HasDownload { get; set; }
        public string DownloadFileName { get; set; }
        public bool HasDraftLoad { get; set; }

        public bool isPayable { get; set; }
        public decimal? Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public bool CreateModeIsWorkflowOn { get; set; }
        public bool AmountIsBasedOnLoanAmount { get; set; }
        public byte[] FSRowVersion { get; set; }
    }

    public class GetFSUpdateResponse : ResponseBase
    {

        public ICollection<LookUpView> Currencies { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
        public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
        public ICollection<LookUpView> IdentificationCardTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> DebtorTypes { get; set; }
        public ICollection<LookUpView> CollateralSubTypes { get; set; }
        public ICollection<LookUpView> CollateralTypes { get; set; }
        public ICollection<LKLGAView> LGADetailed { get; set; }
        public FSView FSView { get; set; }
        public int? EditOfOriginalFinancialStatementID { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public bool HasDownload { get; set; }
        public string DownloadFileName { get; set; }
        public bool HasDraftLoad { get; set; }

        public bool isPayable { get; set; }
        public decimal? Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public bool CreateModeIsWorkflowOn { get; set; }
        public bool AmountIsBasedOnLoanAmount { get; set; }
        public byte[] FSRowVersion { get; set; }
    }

}
