using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Payments;

namespace CRL.Model.ModelViews.Memberships
{
    public class ClientView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientType { get; set; }
        public string RepresentativeClient { get; set; }
        public int? RepresentativeClientId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }



    }

   
}
