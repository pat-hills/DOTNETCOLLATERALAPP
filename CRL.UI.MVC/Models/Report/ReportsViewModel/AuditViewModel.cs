using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views;
using CRL.Model.ModelViews.Administration;
using CRL.Infrastructure.Helpers;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class _AuditReportViewModel : ReportBaseViewModel
    {
        public _AuditReportViewModel()
        {


        }
        public ICollection<AuditView> AuditViews { get; set; }
        [Display(Name = "Audit category")]
        public int[] AuditType { get; set; }
        [Display(Name = "Audit action")]
        public string Action { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "IP / machine name")]
        public string Machiname { get; set; }
        [Display(Name = "User's Institution")]
        public int? ClientId { get; set; }
        [Display(Name = "User")]
        public string UserName { get; set; }
        [Display(Name = "User Login Id")]
        public string UserLoginId { get; set; }
        public int? UserId { get; set; }
        [Display(Name = "Limit to my audits")]
        public bool LimitTo { get; set; }
        [Display(Name = "Audit Action")]
        public int?[] AuditAction { get; set; }

        public List<SelectListItem> AuditTypeList { get; set; }
        public List<AuditActionTypeView> AuditActionList { get; set; }
        public List<SelectListItem> Clients { get; set; }


    }
}