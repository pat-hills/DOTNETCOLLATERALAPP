using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Messaging
{
  

    public class ViewInstitutionUnitsResponse:ResponseBase
    {
        public ICollection<InstitutionUnitGridView> InstitutionUnitGridView { get; set; }
        public int NumRecords { get; set; }


    }
}
