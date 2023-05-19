using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Model.FS.Enums
{
    public enum FinancialStatementLoanCategory { Loan = 1, LineOfCredit = 2, Both = 3 }
    public enum FinancialStatementTransactionCategory
    {
        [XmlEnum("1")]
        SecurityInterest = 1,
        [XmlEnum("2")]
        FinancialLease = 2,
        [XmlEnum("3")]
        Lien = 3
    }


}
