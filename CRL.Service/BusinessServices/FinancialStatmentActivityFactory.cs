using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Model.FS;
using CRL.Model.Messaging;

namespace CRL.Service.BusinessServices
{
    public static class FinancialStatementActivityFactory
    {
        public static FinancialStatementActivity GetFinancialStatementActivity(FSActivityRequest request)
        {
            FinancialStatementActivity fsActivity;  //Use factory to get it depending on the node

             if (request.GetType() == typeof(UpdateFSRequest)) // returns false
             {
                 fsActivity = new ActivityUpdate();
                 return fsActivity;

             }
             else if (request.GetType() == typeof(RenewalFSRequest)) // returns false
             {
                 fsActivity = new ActivityRenewal();
                 return fsActivity;
             }
             else if (request.GetType() == typeof(DischargeFSRequest)) // returns false
             {
                 fsActivity = new ActivityDischarge();
                 return fsActivity;
             }
             else if (request.GetType() == typeof(SubordinateFSRequest )) // returns false
             {
                 fsActivity = new ActivitySubordination();
                 return fsActivity;
             }
             else if (request.GetType() == typeof(AssignFSRequest  )) // returns false
             {
                 fsActivity = new ActivityAssignment ();
                 return fsActivity;
             }
             else
             {
                 throw new Exception("Wrong activity type");
             }

        }
    }
}
