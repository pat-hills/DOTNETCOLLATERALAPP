using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Model.ModelViews;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.Memberships;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class SelectRegisteredClientViewModel
    {
       

        public SelectList LegalEntityClientList { get; set; }
        public SelectList IndividualClientList { get; set; }
        public ClientSummaryView ClientSummaryView { get; set; }
        public int SelectedLegalEntityMembershipId { get; set; }       
        public int SelectedIndividualMembershipId { get; set; }
        public bool RegisteredOrPublicUser { get; set; }
        public string SearchClientCode { get; set; }
        public int SearchByClientCodeOrName { get; set; }
        public int?  IsSelectedIndividual { get; set; }
        public string ContinueLabel { get; set; }
        public int? FinancialStatementId { get; set; }
        public FinancialStatementActivityCategory SelectedAmendmentType { get; set; }
    }
}