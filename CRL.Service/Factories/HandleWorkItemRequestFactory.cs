using CRL.Model.Messaging;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Service.Common;
using CRL.Service.Interfaces.FinancialStatement;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.Workflow.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Factories
{
    public static class   HandleWorkItemRequestFactory 
    {
        public static HandleWorkItemRequest CreateHandleWorkItemRequest(HandleWorkItemRequest request)
        {
            if (request.wfTaskType == WFTaskType.CreateRegistration)
            {
                
                return new NewFSRequest();
            }
            else if (request.wfTaskType == WFTaskType.UpdateRegistration)
            {
                
                return new UpdateFSRequest() { FinancialStatementActivityType = FS.Enums.FinancialStatementActivityCategory.Update };
                
            }
            else if (request.wfTaskType == WFTaskType.DischargeRegistration)
            {
                
                return new DischargeFSRequest() { FinancialStatementActivityType = FS.Enums.FinancialStatementActivityCategory.FullDicharge };

            }
            else if (request.wfTaskType == WFTaskType.DischargeRegistrationDueToError)
            {

                return new DischargeFSDuetoErrorRequest() { FinancialStatementActivityType = FS.Enums.FinancialStatementActivityCategory.DischargeDueToError };
            
            }
            else if (request.wfTaskType == WFTaskType.SubordinateRegistration)
            {

                return new SubordinateFSRequest() { FinancialStatementActivityType = FS.Enums.FinancialStatementActivityCategory.Subordination };

            }
            else if (request.wfTaskType == WFTaskType.AssignRegistration)
            {

                return new AssignFSRequest() { FinancialStatementActivityType = FS.Enums.FinancialStatementActivityCategory.FullAssignment };

            }

            return null;

        }
    }
}
