using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.WorkflowEngine
{
    public class WorkflowTemplate
    {
        #region Properties
        public string EventDate { get; set; }
        public string EventNo { get; set; }
        public string EventTitle { get; set; }
        public string CurrentPendingStatus { get; set; }
        //public string CurrentPendingTasks { get; set; }
        public string CurrentTaskAssignedTo { get; set; }
        public string CurrentTaskDescription { get; set; }
        public string LastTaskDate { get; set; }  //Please use the last token's date
        //public string EventTaskPerformed { get; set; }  //Please use all active workitem's roles
        public string LastTaskPerformedBy { get; set; }
        public string LastTaskPerformed { get; set; }
        public string LastTaskComment { get; set; }  //Please use the current place name     
        public string validationError { get; set; }
        #endregion Properties

        public void LoadTemplate(WFCase _case)
        {
            bool isNewSubmissionFlag;
            //Get the event date
            this.EventDate = _case.CreatedOn.ToString("dd-MMM-yyyy");

            //Get the event no
            this.EventNo = _case.CaseContext;

            //Get the event title
            this.EventTitle = _case.CaseTitle ;

            //Get the current status of the event|| s.TokenStatus == "CONS"
            var lastToken =  _case.Tokens .Where (s=>s.TokenStatus == "FREE" || s.TokenStatus == "LOCK" ).OrderByDescending  (d=>d.CreatedOn).FirstOrDefault ();
            if (lastToken != null)
            {
                this.CurrentPendingStatus = lastToken.Place.Name;
                this.CurrentTaskDescription = lastToken.Place.Description;
            }

            
            //Now we need to extract all users assigned to this task
            foreach (var users in lastToken.AssignedUsers )
            {
                this.CurrentTaskAssignedTo += NameHelper.GetFullName(users.FirstName, users.MiddleName, users.Surname);
            }

            if (this.CurrentTaskAssignedTo == null)
            {
                this.CurrentTaskAssignedTo = "No Users currently assigned to handle this"; //
                this.validationError = "No Users currently assigned to handle this";
            }
            else
            {
                this.CurrentTaskAssignedTo = this.CurrentTaskAssignedTo.Trim().TrimEnd(',');
            }


          


            //History list is to take all finished workitems
            var lastExecutedWorkItem = _case.WorkItems.Where(s=>s.WorkitemStatus == "FI"  ).OrderByDescending (d=>d.CreatedOn ).FirstOrDefault ();
            if (lastExecutedWorkItem != null)
            {
                this.LastTaskComment = lastExecutedWorkItem.ExecutingUserComment ;
                this.LastTaskPerformedBy = NameHelper.GetFullName(lastExecutedWorkItem.ExecutingUser.FirstName, lastExecutedWorkItem.ExecutingUser.MiddleName, lastExecutedWorkItem.ExecutingUser.Surname);
                this.LastTaskPerformed = lastExecutedWorkItem.Transition.ActionTakenName;
                this.LastTaskDate = lastExecutedWorkItem.FinishedDate.Value.ToString("dd-MMM-yyyy");
            }



        }
    }

     

}
