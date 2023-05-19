using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views.Payments;

using CRL.Model.ModelViews.Payments;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class PaymentsViewModel : ReportBaseViewModel
    {
        public PaymentsViewModel():base()
        {
            PaymentTypeList = new List<SelectListItem>();
            PaymentTypeList.Add(new SelectListItem { Text = "Normal", Value = "1" });
            PaymentTypeList.Add(new SelectListItem { Text = "Reversal", Value = "2" });

            PublicUserList = new List<SelectListItem>();
            //PublicUserList.Add(new SelectListItem { Text = "Registered clients", Value = "1" });
            PublicUserList.Add(new SelectListItem { Text = "Financial Institution clients", Value = "2" });
            //PublicUserList.Add(new SelectListItem { Text = "Individual clients only", Value = "3" });
            PublicUserList.Add(new SelectListItem { Text = "Public users", Value = "4" });

            PaymentSourceList = new List<SelectListItem>();
            //PaymentSourceList.Add(new SelectListItem { Text = "Paypoint", Value = "1" });
            PaymentSourceList.Add(new SelectListItem { Text = "Settlement", Value = "2" });
            PaymentSourceList.Add(new SelectListItem { Text = "Interswitch Webpay", Value = "3" });
            PaymentSourceList.Add(new SelectListItem { Text = "Interswitch DirectPay", Value = "4" });


           

        }
        public ICollection<PaymentView> PaymentsView { get; set; }
        public List<SelectListItem> PaymentTypeList { get; set; }
        public List<SelectListItem> PaymentSourceList { get; set; }
        public List<SelectListItem> PublicUserList { get; set; }
           [Display(Name = "Payment No")]
        public string PaymentNo { get; set; }
           [Display(Name = "Payment Type")]
        public int? PaymentTypeId { get; set; }
           [Display(Name = "Payment Source")]
           public int? PaymentSourceId { get; set; }
           [Display(Name = "Transaction No")]
        public string TransactionNo { get; set; }
        public int? MembershipId { get; set; }
         [Display(Name = "Paid by")]
        public string Payee { get; set; }
           [Display(Name = "Client Type")]
        public short? IsPublicUser { get; set; }
           [Display(Name = "Public User Email")]
        public string PublicUserEmail { get; set; }
        [Display(Name="Received By")]
        public string PaypointUserName { get; set; }
        [Display(Name = "Public User Security Code")]
        public string PublicUserSecurityCode { get; set; }
           [Display(Name = "Paypoint")]
        public string PaypointName { get; set; }
           [Display(Name = "Paid by client")]
           public string ClientName { get; set; }
        public int ShowType { get; set; }
       



    }
}