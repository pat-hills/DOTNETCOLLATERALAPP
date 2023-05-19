using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.FinancialStatement
{
    [Serializable]
    public class IndividualParticipantSummaryView : ParticipantSummaryView
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }          
        public int? PersonIdentificationTypeId { get; set; }
        public string PersonIdentificationTypeName { get; set; }    
    }
}
