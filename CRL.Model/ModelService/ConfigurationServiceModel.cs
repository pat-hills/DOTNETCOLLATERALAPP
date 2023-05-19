using CRL.Infrastructure.Authentication;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Model.FS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelService
{
    public class ConfigurationWorkflowServiceModel
    {
       IConfigurationWorkflowRepository _configurationWorkflowRepository{get;set;}
        SecurityUser _executingUser{get;set;}
        public ConfigurationWorkflow activeconfigWorkflow{get;set;}
        ConfigurationWorkflow institutionConfigWorkflow;
        ConfigurationWorkflow globalConfigWorkflow;
        
        public ConfigurationWorkflowServiceModel(IConfigurationWorkflowRepository configurationWorkflowRepository, SecurityUser user)
       {
           _configurationWorkflowRepository = configurationWorkflowRepository ;
           _executingUser = user;
       }
       public  void LoadInitialDataFromRepository()
       {
           //Load the workflow configuration
           var configWorkflows = _configurationWorkflowRepository.GetDbSet().Where(s => s.MembershipId == _executingUser.MembershipId || s.MembershipId == null).ToList ();
           institutionConfigWorkflow = configWorkflows.Where(s => s.MembershipId == _executingUser.MembershipId).SingleOrDefault();
           globalConfigWorkflow = configWorkflows.Where(s => s.MembershipId == null).Single ();
          
           if (institutionConfigWorkflow == null || institutionConfigWorkflow .UseGlobalSettings )         
              activeconfigWorkflow=globalConfigWorkflow;               
           else
               activeconfigWorkflow = institutionConfigWorkflow;
              
           
       }

        public bool WorkflowOnForCreateNewFS()
        {
            if (activeconfigWorkflow.CreateNewFS == true)
            return true;else return false;        

        }
        
        public bool WorkflowOnForFSActivity(FinancialStatementActivityCategory fsA)
        {
            bool Result = false;
            if (fsA == FinancialStatementActivityCategory.Update)
                Result = activeconfigWorkflow.UpdateFS;
            else if (fsA == FinancialStatementActivityCategory.Subordination)
                Result= activeconfigWorkflow.SubordinateFS ;
            else if (fsA == FinancialStatementActivityCategory.PartialDischarge )
                Result = activeconfigWorkflow.DischargeFS ;
            else if (fsA == FinancialStatementActivityCategory.FullDicharge )
                Result = activeconfigWorkflow.DischargeFS;
            else if (fsA == FinancialStatementActivityCategory.DischargeDueToError)
                Result = activeconfigWorkflow.DischargeFS;
            else if (fsA == FinancialStatementActivityCategory.FullAssignment )
                Result = activeconfigWorkflow.AssignmentFS;
            else return false;

            return Result;
        }
    }
}
