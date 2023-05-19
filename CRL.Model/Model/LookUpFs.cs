using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.FS
{
    public class LookUpForFS
    {
        public ICollection<LookUpView> CollateralTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> CollateralSubTypes { get; set; }
        public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
        public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
        public ICollection<LookUpView> Currencies { get; set; }
        public ICollection<LookUpView> IdentificationCardTypes { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
        public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
        public ICollection<LookUpView> DebtorTypes { get; set; }
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> LGAs { get; set; }
        public ICollection<LookUpView> RegistrationNoPrefix { get; set; }
        public ICollection<LookUpView> FinancialStatementActivityType { get; set; }

    }
}
