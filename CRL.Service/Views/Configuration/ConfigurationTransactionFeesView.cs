using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Configuration;
using System.ComponentModel.DataAnnotations;
using CRL.Model.Payments;

namespace CRL.Service.Views.Configuration
{
    public class ConfigurationTransactionFeesView
    {
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Service Id provided")]
        public int? Id { get; set; }
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Service Fee Type provided")]
        public ServiceFees ServiceTypeId { get; set; }
        public LKServiceFeeCategory ServiceType { get; set; }
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Service Fee provided")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal Fee { get; set; }
        public int LenderTypeId { get; set; }
    }
}
