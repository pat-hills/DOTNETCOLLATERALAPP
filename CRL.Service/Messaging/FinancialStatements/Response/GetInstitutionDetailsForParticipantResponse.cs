
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.FinancialStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
   public class GetInstitutionDetailsForParticipantResponse:ResponseBase
    {
       public InstitutionSPView institution { get; set; }
    }


   
}
