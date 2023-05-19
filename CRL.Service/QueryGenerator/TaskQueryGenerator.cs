using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Views.Workflow;
using CRL.Model.WorkflowEngine.Enums;

namespace CRL.Service.QueryGenerator
{
    public static class TaskQueryGenerator
    {
        public static IEnumerable<TaskGridView> CreateQueryForTask(
          ViewMyTasksRequest request, IWFCaseRepository _rpFS)
        {
            //IQueryable<WFCase> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.CaseStatus=="OP");

         int[] UserId={request .SecurityUser .Id };
         //query = query.Where(s => s.WorkItems.Where(wk => wk.IsActive == true && wk.IsDeleted == false && wk.WorkitemStatus == "EN").Any(st => st.AssignedUsers.Any(t => UserId.Contains(t.Id))));
         //query = query.Select(d => new TaskGridView
         //{
         //    Id = d.Id,
         //    CaseTitle = d.CaseTitle,
         //    Placename = d.Tokens.Where(m => m.TokenStatus == "FREE").SingleOrDefault().Place.Name,
         //    CreatedOn = d.CreatedOn,
         //    SubmittedBy = d.WorkItems.Where(z => z.WorkitemStatus == "FI").
         //        Select(dd => NameHelper.GetFullName(dd.ExecutingUser.FirstName, dd.ExecutingUser.MiddleName, dd.ExecutingUser.Surname)).LastOrDefault()
         //});


         var db = from a in _rpFS.GetDbSet()
                  where a.IsDeleted == false && a.IsActive == true && a.CaseStatus == "OP" &&  (request .CaseType ==null || (a.CaseType==(WorkflowRequestType? )request.CaseType  )) &&
                  a.Tokens .Where(wk => wk.IsActive == true && wk.IsDeleted == false && wk.TokenStatus  == "FREE").Any(st => st.AssignedUsers.Any(t => UserId.Contains(t.Id)))
                  orderby a.CreatedOn descending
                  select new TaskGridView
      {
          Id = a.Id,
          CaseTitle = a.CaseTitle,
          Placename = a.Tokens.Where(m => m.TokenStatus == "FREE").FirstOrDefault ().Place.Name,
          CreatedOn = a.CreatedOn,
           CaseType= a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatement ?"Registration of Financing Statement":
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatementActivity ? "Amendment or Cancellation of Financing Statement":
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.UpdateFinancingStatement  ? "Update of Financing Statement":
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.SubordinateFinancingStatement  ? "Subordination of Financing Statement":
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.AssignFinancingStatement  ? "Transfer of Financing Statement":
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.CancelFinancingStatement  ? "Cancellation of Financing Statement" :
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.MembershipRegistration  ? "Registration of Client" :
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.Membership ? "Postpaid Account Setup" :
           a.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.PaypointUserAssigment ? "PayPoint Users Request" : "N/A",

          SubmittedBy = a.WorkItems.Where(z => z.WorkitemStatus == "FI").OrderByDescending (m=>m.FinishedDate ).
              Select(dd => dd.ExecutingUser.FirstName + " " + dd.ExecutingUser.MiddleName + " " + dd.ExecutingUser.Surname).FirstOrDefault()
      };
        
            if(request.CreatedRange != null)
            {
                db = db.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }

            if (request.SubmittedBy != null)
            {
                db = db.Where(s => s.SubmittedBy.Contains(request.SubmittedBy));
            }

         return db.ToList();
            



      

         

           

        }
    }
}
