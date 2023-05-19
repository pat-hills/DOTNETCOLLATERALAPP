using CRL.Infrastructure.Messaging;
using CRL.Model.FS.Enums;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Messaging
{
    public class FSRequest : HandleWorkItemRequest
    {
        public FSView FSView { get; set; }

    }
    public class FSActivityRequest : FSRequest 
        {
            public int Id { get; set; }
            public int FinancialStatementId { get; set; }
                       
            public int[] AssignedUsers { get; set; } //Represent the users 
            public string[] NotifiedEmails { get; set; }
           
            public FinancialStatementActivityCategory FinancialStatementActivityType { get; set; }

            public byte[] FSRowVersion { get; set; }



        }

        public class SubordinateFSRequest : FSActivityRequest
        {

            public int SubordinateToMembershipId { get; set; }
            public SubordinatingPartyView SubordinatingPartyView { get; set; }

        }


        public class AssignFSRequest : FSActivityRequest
        {
            public int AssignType { get; set; }
            public int AssignToMembershipId { get; set; }
            public string AssignmentDescription { get; set; }


        }

        public class UpdateFSRequest : FSActivityRequest
        {
           
            public bool HasDownload { get; set; }
        }

        public class RenewalFSRequest : FSActivityRequest
        {
            public DateTime ExpiryDate { get; set; }

        }
        public class DischargeFSRequest : FSActivityRequest
        {
            public DischargeFSRequest()
            {
                DischargeType = 1; //**Remove when doing partial discharge and rather find out the kind of dicharge it is
            }
            public int DischargeType { get; set; }
            public int[] PartiallyDischargedCollateralIds { get; set; }
        }

        public class DischargeFSDuetoErrorRequest : FSActivityRequest { 
        
        }
    
}
