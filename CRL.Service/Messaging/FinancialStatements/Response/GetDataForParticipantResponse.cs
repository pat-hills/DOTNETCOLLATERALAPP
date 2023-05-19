using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetDataForParticipantResponse
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
        public ParticipantView ParticipantView { get; set; }
    }
}
