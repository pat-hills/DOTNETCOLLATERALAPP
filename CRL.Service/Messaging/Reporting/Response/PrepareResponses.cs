using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.Reporting.Response
{
    public class PrepareClientExpenditureReportResponse : ResponseBase
    {
        public ICollection<LookUpView> Institutions { get; set; }
        public ICollection<LookUpView> Units { get; set; }
           
    }
    public class PrepareClientStatReportResponse : ResponseBase
    {
      
        public ICollection<LookUpView> Clients { get; set; }
  
    }
    public class PrepareAuditReportResponse : ResponseBase
   {
         public ICollection<LookUpView> Users { get; set; }      
         public ICollection<LookUpView> AuditTypes { get; set; }
         public ICollection<AuditActionTypeView> AuditActionTypes { get; set; }
         public ICollection<LookUpView> Clients { get; set; }
         public ICollection<LookUpView> SecuredPartyList { get; set; }
   }
   public class PrepareCreditActivitiesReportResponse:ResponseBase
   {
         public ICollection<LookUpView> AccountTransactionTypes { get; set; }      
         public ICollection<LookUpView> ServiceFees { get; set; }  
   }

   public class PrepareCustomQueryReportResponse : ResponseBase
   {
       //public ICollection<LookUpView> CollateralTypes { get; set; }
       public ICollection<LookUpView> Currencies { get; set; }  //Gets the type of institution
       //public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
       public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
       public ICollection<LookUpView> SectorsOfOperation { get; set; }
       //public ICollection<LookUpView> IdentificationCardTypes { get; set; }  //Gets the type of institution
       public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
       public ICollection<LookUpView> DebtorTypes { get; set; }
       public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
       public ICollection<LookUpView> Countries { get; set; }
       public ICollection<LookUpView> Countys { get; set; }
       public ICollection<LookUpView> CollateralSubTypes { get; set; }

       public ICollection<LookUpView> Clients { get; set; }
   }
    
}
