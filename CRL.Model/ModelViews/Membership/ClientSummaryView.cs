using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class ClientSummaryView
    {
          [Display(Name = "Secured Creditor Code")]
        public string ClientCode { get; set; }
          [Display(Name = "Name")]
        public string ClientName { get; set; }
          [Display(Name = "Email")]
        public string ClientEmail { get; set; }
          [Display(Name = "Phone")]
        public string ClientPhone { get; set; }
         [Display(Name = "Client Type")]
        public int ClientIsIndividualOrLegal { get; set; }
         [Display(Name = "Client Type")]
         public string  ClientType { get; set; }
          [Display(Name = "Client is postpaid")]
        public bool ClientIsPostPaidOrPrepaid { get; set; }
        public int MembershipId { get; set; }
        public decimal Balance { get; set; }

    }
}
