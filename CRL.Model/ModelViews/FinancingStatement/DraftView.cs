using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.FinancingStatement
{
    public class DraftView
    {
        public int Id { get; set; }
        public short RegistrationOrUpdate { get; set; }
        public short CreateOrEditMode { get; set; }
        public int AssociatedIdForNonNew { get; set; }
        public string ServiceRequest { get; set; }
        public string Name { get; set; }
        public string ActionWhenDraftWasCreated { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
