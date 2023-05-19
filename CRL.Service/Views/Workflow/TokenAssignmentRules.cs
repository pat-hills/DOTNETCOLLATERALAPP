using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Configuration;

namespace CRL.Service.Views.Workflow
{
    public class TokenAssignmentRulesView
    {
        public int TokenId { get; set; }
        public ICollection<WorkflowPlaceAssignmentConfiguration> WorkflowPlaceAssignmentConfiguration { get; set; }
        public int InstitutionId { get; set; }
    }
}
