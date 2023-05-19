using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRL.Service.Views;
using CRL.Service.Views.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class InterSwitchTransactionDetailsViewModel
    {
        public InterSwitchUserView InterSwitchUserView { get; set; }
        [Display(Name = "Amount")] 
        public decimal amount { get; set; }
        [Display(Name = "Currency")] 
        public string currency { get; set; }
        [Display(Name = "Product Id")] 
        public int product_id { get; set; }
        [Display(Name = "Site Redirect Url")] 
        public string site_redirect_url { get; set; }
        [Display(Name = "Transaction Reference")] 
        public string txn_ref { get; set; }
        [Display(Name = "Hash")] 
        public string hash { get; set; }
        [Display(Name = "Pay Item Id")] 
        public string pay_item_id { get; set; }
        [Display(Name = "Customer Id")] 
        public string cust_id { get; set; }
        [Display(Name = "Pay Item Name")] 
        public string pay_item_name { get; set; }
        [Display(Name = "Customer Name")] 
        public string cust_name { get; set; }
    }
}