using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class GetAddParticipantRequest
    {

        public EditMode EditMode { get; set; }
        public bool CreateOrEditModeIsUpdate { get; set; }
        public bool CreateModeIsLien { get; set; }
        /// <summary>
        /// Will treat create as submit and update as update submit
        /// </summary>
        public bool CreateModeIsWorkflowOn { get; set; }
        public int IndvDebSecInsDebSec { get; set; }
        public string ClientCode { get; set; }

    }

    public class GetAddParticipantResponse:ResponseBase
    {
        public ICollection<LookUpView> SectorsOfOperation { get; set; }
        public ICollection<LookUpView> IdentificationCardTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> SecuringPartyIndustryTypes { get; set; }
        public ICollection<LookUpView> DebtorTypes { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> LGAs { get; set; }
        public ICollection<LookUpView> BusinessPrefixes { get; set; }
        public ICollection<LKLGAView> LGAdetailed { get; set; }
        public ParticipantView ParticipantView { get; set; }
    }
}
