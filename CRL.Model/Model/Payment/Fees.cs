using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.Payments
{
   
    public enum ServiceFees
    {
        NewFinancingStatement = 1,
        UpdateOfFinancingStatement = 2,
        DischargeofFinancingStatement = 3,      
        SubordinationOfFinancingStatement = 4,      
        AssignmentOfFinancingStatement = 5,
        Search = 7,
        PublicSearch = 8,
        CertifiedSearchResult=6,
        UnCertifiedSearchResult = 9,
        CertifiedSearchResultPublic = 10,
        UnCertifiedSearchResultPublic = 11,




    };
    public class Fee : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool AllowPostPaidForPostPaidClients { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
