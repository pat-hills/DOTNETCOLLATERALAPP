using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class UsersViewModel: ReportBaseViewModel
    {
        public UsersViewModel()
            : base()
        {
            UserListOptions = new List<SelectListItem>();
            UserListOptions.Add(new SelectListItem { Text = "Registry users only", Value = "1" });
            UserListOptions.Add(new SelectListItem { Text = "Client users only", Value = "2" });

            AccountStatusList = new List<SelectListItem>();
            AccountStatusList.Add(new SelectListItem { Text = "Active", Value = "1" });
            AccountStatusList.Add(new SelectListItem { Text = "Active And Locked", Value = "4" });
            AccountStatusList.Add(new SelectListItem { Text = "Deactivated", Value = "2" });
            AccountStatusList.Add(new SelectListItem { Text = "Deleted", Value = "3" });
            
}

        public ICollection<UserGridView> UsersView { get; set; }
        public DateRange EntryDate { get; set; }
         [Display(Name = "Login Id")]
        public string Username { get; set; }
         [Display(Name = "Name")]
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
          [Display(Name = "Financial Institution")]
        public string ClientName { get; set; }
          //[Display(Name = "Financial Institution Unit")]
          //public string UnitName { get; set; }
        [Display(Name = "Client code")]
          public string ClientCode { get; set; }
        public int? InstitutionUnitId { get; set; }
        public int? InstitutionId { get; set; }  //Required
        public int? MembershipId { get; set; }
          [Display(Name = "Paypoint status")]
        public bool IsPaypointUser { get; set; }
        public bool LoadOnlyIndividualClients { get; set; }
        [Display(Name="User Type")]
        public int? UserListOption { get; set; }
        public List<SelectListItem> UserListOptions;
        
        [Display(Name = "Status")]
        public int[] AccountStatus { get; set; }
        public List<SelectListItem> AccountStatusList { get; set; }

        
    }
}