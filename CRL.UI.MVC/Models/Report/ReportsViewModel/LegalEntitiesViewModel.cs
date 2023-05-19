using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class LegalEntitiesViewModel : ReportBaseViewModel
    {
        public LegalEntitiesViewModel()
            : base()
        {
            StatusTypeList = new List<SelectListItem>();
            StatusTypeList.Add(new SelectListItem { Text = "Active", Value = "0" });
            StatusTypeList.Add(new SelectListItem { Text = "Deactivated", Value = "1" });
            StatusTypeList.Add(new SelectListItem { Text = "Deleted", Value = "2" });
            StatusTypeList.Add(new SelectListItem { Text = "Pending", Value = "3" });
            StatusTypeList.Add(new SelectListItem { Text = "Approved", Value = "4" });
            StatusTypeList.Add(new SelectListItem { Text = "Denied", Value = "5" });
            
}
        public ICollection<InstitutionGridView> InstitutionsView { get; set; }
        [Display(Name="Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Client code")]
        public string ClientCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Display(Name = "Secured creditor type")]
        public int?[] SecuredPartyType { get; set; }
        public List<SelectListItem> SecuredPartyList { get; set; }
        [Display(Name = "Status")]
        public int? StatusType { get; set; }
        public List<SelectListItem> StatusTypeList { get; set; }

    }
}