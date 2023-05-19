using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Payments
{
    public class AccountBatchView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string Comment { get; set; }
        public string BatchStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public bool isReconciled { get; set; }
        public bool ConfirmSubPostpaidAccount { get; set; }
        public decimal? TotalExpenses { get; set; }
        public decimal? TotalSettlement { get; set; }
        

    }
}
