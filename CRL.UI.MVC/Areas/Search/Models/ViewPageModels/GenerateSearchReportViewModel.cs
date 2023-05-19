using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
    public class GenerateSearchReportViewModel
    {
        public bool isNonLegalEffect { get; set; }
        public int SearchId { get; set; }
        public int SelectedFSToSearch { get; set; }
        public int SearchResultCount { get; set; }
          [Display(Name = "Report Type")]
        public bool IsCertified { get; set; }
        public bool SendAsMail { get; set; }
        [Display(Name = "Payment Receipt No")]
        public string PublicUserReceiptNo { get; set; }
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string PublicUserEmail { get; set; }
        public string UniqueIdentifier { get; set; }
        public string publicUserCode { get; set; }

        //Add also finacial Activity
        public bool isPayable { get; set; }
        public decimal Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
    }
}