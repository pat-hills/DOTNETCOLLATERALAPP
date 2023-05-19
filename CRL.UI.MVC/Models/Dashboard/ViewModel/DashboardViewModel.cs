using CRL.Model.ModelViews.Administration;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Models.Dashboard.ViewModel
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        { 
            FSGridViewModel = new List<FSGridView>();
            TaskGridView = new List<TaskGridView>();
            AuditViewModel = new  List<AuditView>();
            MessagesView = new List<MessagesView>();
        }

        public List<FSGridView> FSGridViewModel { get; set; }
        public List<TaskGridView> TaskGridView { get; set; }
        public List<AuditView> AuditViewModel { get; set; }
        public List<MessagesView> MessagesView { get; set; }
        public decimal TotalNoOfFS { get; set; }
    }
}