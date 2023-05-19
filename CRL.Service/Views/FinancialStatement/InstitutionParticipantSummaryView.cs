using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.FinancialStatement
{
    public class InstitutionParticipantSummaryView:ParticipantSummaryView
    {
        public string Name { get; set; }
        public int CompanyTypeId { get; set; }
        public string CompanyTypeName { get; set; }
        public string CompanyNo { get; set; }
        public string BusinessTin { get; set; }
       
        public string GenderComposition { get; set; }
        public int? SecuringPartyIndustryTypeId { get; set; }
        public string SecuringPartyIndustryName { get; set; }
    }
}
