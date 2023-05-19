using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.FinancialStatement
{
    public class ParticipantSummaryView
    {
        public int Id { get; set; }
        public bool Lead { get; set; }
        [Display(Name = "Type")]
        public string ParticipantTypeName { get; set; }           
        public int ParticipationTypeId { get; set; }
        public int ParticipantTypeId { get; set; }
        
    }
}
