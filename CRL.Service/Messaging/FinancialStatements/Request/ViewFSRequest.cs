using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.Common.Enum;
using CRL.Model.FS.Enums;

using CRL.Service.Messaging.Reporting.Request;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Request
{


    public class ViewFSCustomQueryRequest : ReportRequestBase
    {
        public DateRange RegistrationDate { get; set; }
        public FinancialStatementTransactionCategory[] FinancialStatementTransactionTypeId { get; set; }
        public System.Nullable<FinancialStatementLoanCategory>[] FinancialStatementLoanTypeId { get; set; }
        public CollateralCategory[] CollateralTypeId { get; set; }
        public int[] CollateralSubTypeId { get; set; }
        public System.Nullable<DebtorCategory>[] DebtorTypeId { get; set; }
        public int?[] SecuredPartyTypeId { get; set; }
        public int?[] DebtorCountryId { get; set; }
        public int?[] DebtorCountyId { get; set; }
        public int?[] DebtorNationalityId { get; set; }
        public int?[] SectorOfOperationId { get; set; }
        public int?[] MaximumCurrency { get; set; }     
        public short? MajorityOwnershipId { get; set; }      
        public short? ExistingRelationshipId { get; set; }
        public GroupByNoOfFSStat GroupBy { get; set; }
        public int ShowListOrStatistical { get; set; }
        public int[] IncludeSections { get; set; }
        public bool LimitToFirstDebtor { get; set; }
        public bool LimitToFirstSecuredParty {get;set;}
        public int FSState { get; set; }
        public int FSStateType { get; set; }
        public int? ClientId { get; set; }
        
    }

}
