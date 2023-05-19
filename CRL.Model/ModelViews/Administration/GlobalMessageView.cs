using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.ModelViews.Administration
{
    public class GlobalMessageView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class GlobalMessageDetailsView : GlobalMessageView
    {
        public MessageCategory MessageTypeId { get; set; }
        public bool IsLimitedToAdmin { get; set; }
        public short? IsLimitedToClientOrOwners { get; set; }
        public short? IsLimitedToInstitutionOrIndividual { get; set; }
    }
}
