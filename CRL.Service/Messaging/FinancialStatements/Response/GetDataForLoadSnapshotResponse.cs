using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Service.Views;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;
using CRL.Infrastructure.Helpers;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetDraftResponse : ResponseBase
    {
        public FSView FSView { get; set; }
        public EditMode EditMode { get; set; }
        public short RegistrationOrUpdate { get; set; }

        public int AssociatedCaseIdForEdit { get; set; }
        public int AssociatedWorkItemForEdit { get; set; }
        public int AssociatedFsActivityForUpdateEdit { get; set; }
        public int? AssociatedOriginalFinancialStatementIdForUpdateEdit { get; set; }
        public string DraftName { get; set; }

       public string UniqueIdentifierNo { get; set; }
       
    }

    public class ViewDraftResponse : ResponseBase
    {
        public FSView FSView { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }

    }
}
