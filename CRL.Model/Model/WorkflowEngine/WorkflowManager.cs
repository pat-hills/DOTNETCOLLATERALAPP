using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Domain;
using CRL.Model.Memberships;

namespace CRL.Model.WorkflowEngine
{
    //Shall process the workflow
    public class WorkflowManager
    {
        private WFWorkflow _workflow;
        private WFCase _case;
        public  User ExecutingUser { get; set; }
        public string UserComment { get; set; }
        public AuditingTracker _tracker { get; set; }
        public WorkflowManager(WFWorkflow workflow, AuditingTracker tracker)
        {
            _workflow = workflow;
            _tracker = tracker;
        }
        public WorkflowManager(WFCase pcase, AuditingTracker tracker)
        {
            _case = pcase;
            _workflow = pcase.Workflow;
            _tracker = tracker;
        }
        public WFCase ProcessNewCase(string objectId, WFCase Case)
        {
            //Workflow must be initiated
            if (_workflow == null)
                throw new Exception("Cannot process a null workflow");


            _case = Case;
            _case.Workflow = _workflow;
            _case.CaseContext = objectId;
            _tracker.Created.Add(_case);

            //Does this workflow have a starting place
            WFPlace wfStartPlace = _workflow.GetStartingPlace();
            if (wfStartPlace == null)
                throw new Exception("No starting place defined for Workfow with ID:" + _workflow.Id + ". Please contact CRG Administrator!");

            this.CreateNewToken(wfStartPlace);

            return _case;
        }

        public void PerformWorkItem(WFWorkItem workitem)
        {

            //this.ExecuteWorkItemTask(workitem);
            this.FinishWorkItem(workitem);
        }



        private void CreateNewToken(WFPlace place)
        {
            //If place is starting place
            WFToken token = new WFToken();
            _tracker.Created.Add(token);

            _case.Tokens.Add(token);
            token.Place = place;
            token.TokenContext = _case.CaseContext;

            if (_case.LimitedToOtherMembershipId != null)
                token.LimitToMembershipId = _case.LimitedToOtherMembershipId;         

            if (_case.LimitedToOtherUnitId != null)
            {
                token.LimitToUnitId = _case.LimitedToOtherUnitId;
            }
           




            //Check if the item is to be lkocked for this place
            if (place.LockItemInThisPlace)
            {
                _case.LockItem = true;
            }
            else
            {
                _case.LockItem = false;
            }


            if (place.PlaceType == 9) //If the token is placed at the end
            {
                token.TokenStatus = "CONS";
                token.TokenConsumedDate = DateTime.Now;

                // Mark any outstanding token as cancelled
                foreach (var tokenitem in _case.Tokens.Where(p => p.TokenStatus == "FREE" || p.TokenStatus == "LOCK"))
                {

                    tokenitem.TokenStatus = "CONS";
                    tokenitem.TokenConsumedDate = DateTime.Now;
                    _tracker.Updated.Add(tokenitem);


                }

                // Mark any outstanding workitems as cancelled
                foreach (var worrkitem in _case.WorkItems.Where(p => p.WorkitemStatus == "EN" || p.WorkitemStatus == "IP"))
                {

                    worrkitem.WorkitemStatus = "CA";
                    worrkitem.CancelledDate = DateTime.Now;
                    _tracker.Updated.Add(worrkitem);


                }

                // Mark this case as closed
                _case.CaseEndDate = DateTime.Now;
                _case.CaseStatus = "CL";
                _tracker.Updated.Add(_case);
            }



            //Find all transitions which are joined to this place
            //We do this by checking all arcs loaded with the IN status
            List<WFArc> InwardArcs = place.GetInWardArcs();

            foreach (WFArc c in InwardArcs)
            {
                //Let us examine the arc
                examineInwardArc(c);
            }


        }

        private bool examineInwardArc(WFArc inwardArc)
        {
            //Get the transition which is joined to this inward arc
            WFTransition inwardArcTransition = inwardArc.Transition;



            //Go through each of the transition's inward arcs and check if the correct number of token can be found
            foreach (WFArc c in inwardArcTransition.Arcs.Where(s => s.ArcDirections == "IN"))
            {
                //Check token lists to get the number of tokens which are in this place
                var tokens = from s in _case.Tokens
                             where s.Place == c.Place
                             select s;

                //Get the number of tokens
                int tokencount = tokens.ToList().Count;

                //No tokens at this place
                if (tokencount == 0)
                    return false;

                //Check if there are arcs which are joined to this place and are part of an OR join 
                var or_arcs = from out_orarcs in c.Place.Arcs
                              where out_orarcs.ArcDirections == "OUT" && out_orarcs.ArcType == "OR_JOIN"
                              select out_orarcs;

                int transition_count = or_arcs.ToList().Count;

                if (transition_count > 0)
                    if (tokencount != transition_count)
                    {
                        //not enough tokens, so unable to proceed
                        return false;
                    }
            }

            // transition has enough input tokens, so create a new workitem
            WFWorkItem workitem = new WFWorkItem();
            _tracker.Created.Add(workitem);
            //workitem.worf = (short)WFWorkflow.ID;
            workitem.Transition = inwardArc.Transition;
            _case.WorkItems.Add(workitem);

            //We will later need to set the work tiems based on task;
            //pWorkItem.RoleID = WFTransitions.Single(s => s.ID == inwardArc.TransitionID).RoleID;
            //pWorkItem.OwnerID = this.OwnerID;
            //$workitem_data['task_id'] = $arc_data['task_id'];
            workitem.Context = _case.CaseContext;
            workitem.WorkitemStatus = "EN";
            //workitem.TransitionTrigger = WFTransitions.Single(s => s.ID == inwardArc.TransitionID).TransitionTrigger;
            workitem.TransitionTrigger = inwardArc.Transition.TaskTrigger;

            //if (WFTransitions.Single(s => s.ID == inwardArc.TransitionID).TransitionTrigger == "TIME")
            //{
            //    pWorkItem.DeadLine = pWorkItem.EnabledDate.Value.AddDays((double)WFTransitions.Single(s => s.ID == inwardArc.TransitionID).TimeLimit);

            //} // if

            //If workitem is not auto then please assign the comment to it
            //pWorkItem.Comment = this.WFWorkItemComment;
            //WFWorkItems.Add(pWorkItem);

            if (workitem.TransitionTrigger == "AUTO")
            {
                //Call Execute this workitem with the workitem ID
                //ExecuteWorkItemTask(workitem);
                FinishWorkItem(workitem);
            }
            return true;
        }

        ///// <summary>
        ///// Responsible for executing tasks associated with workflow, . Distributed transaction should be on for transactions to take place
        ///// </summary>
        ///// <param name="WorkItemID"></param>
        //private void ExecuteWorkItemTask(WFWorkItem WorkItem)
        //{
        //    WorkItem.WorkitemStatus = "EN";
        //    WFApplicationTasksBOList TasksToExecute = WFTransitions.Single(s => s.ID == WorkItem.TransitionID).TransitionTasks;

        //    //Generate the parameters
        //    object[] c = new object[] { contextDB, validationErrorsDB, WFParameters };

        //    //Create an instance of the object
        //    Type objectType = typeof(ApplicationTaskExecutor);


        //    foreach (WFApplicationTasksBO tasks in TasksToExecute)
        //    {

        //        //object listObject = Activator.CreateInstance(objectType);
        //        // Grabbing the specific static method
        //        MethodInfo methodInfo = objectType.GetMethod(tasks.ApplicationMethodName, System.Reflection.BindingFlags.Static | BindingFlags.Public);
        //        //Call the method to load the object
        //        methodInfo.Invoke(null, c);

        //    }

        //}

        /// <summary>
        /// Perorms processing after a workitem has been successfully executed
        /// 
        /// </summary>
        public void FinishWorkItem(WFWorkItem finishedWorkItem)
        {
            //int wrkitemID = WorkItem.ID;
            //We will need to differentiate between updates and new created

            string[] workitemStatus = new string[] { "EN", "IP" };
            //var finishedWorkItem = (from c in WFWorkItems
            //                        where c.ID == wrkitemID && workitemStatus.Contains(c.WorkitemStatus)
            //                        select c).Single();



            if (!workitemStatus.Contains(finishedWorkItem.WorkitemStatus))
            {
                //No workitem was really processed must be an error
                return;
            }

            if (!String.IsNullOrWhiteSpace(this.UserComment))
            {
                finishedWorkItem.ExecutingUserComment = this.UserComment;
                this.UserComment = ""; //Will cortrect by diabling auto submit and then we will create and submit workflow

            }
            // mark this workitem as finished
            finishedWorkItem.WorkitemStatus = "FI";
            _tracker.Updated.Add(finishedWorkItem);
            finishedWorkItem.ExecutingUser = ExecutingUser;
            finishedWorkItem.FinishedDate = DateTime.Now;

            //finishedWorkItem.ExecutingUserID = -1; //When we call save we will replace by the real userid
            //$workitem_array['user_id']         = $_SESSION['logon_user_id'];

            // ********************************************************************
            // find tokens on all input arcs (there may be more than one)
            //var ptokens = from d in _case .Tokens 
            //              join e in WFArcs on d.PlaceID equals e.PlaceID
            //              where e.TransitionID == finishedWorkItem.TransitionID && e.ArcDirections == "IN"
            //              select d;
            var ptokens = from d in _case.Tokens
                          join e in finishedWorkItem.Transition.Arcs on d.Place equals e.Place
                          where e.ArcDirections == "IN"
                          select d;



            //require_once 'classes/wf_arc.class.inc' ;
            //$dbarc =& RDCsingleton::getInstance('wf_arc');

            //$dbarc->sql_select = 'wf_arc.*, token_id, case_id, token_status';
            //$dbarc->sql_from   = "wf_arc "
            //                   . "LEFT JOIN wf_token ON (wf_token.case_id=$case_id AND wf_token.workflow_id=wf_arc.workflow_id AND wf_token.place_id=wf_arc.place_id) ";
            //$where = "workflow_id='$workflow_id' AND transition_id='$transition_id' AND direction='IN'";
            //$arc_data = $dbarc->getData($where);
            //if ($dbarc->errors) {
            //    $this->errors = array_merge($this->errors, $dbarc->getErrors());
            //    return;
            //} // if

            // now mark them all as consumed
            foreach (var ptoken in ptokens)
            {
                if (ptoken.TokenStatus == "FREE")
                {
                    ptoken.TokenStatus = "CONS";
                    ptoken.TokenConsumedDate = DateTime.Now;

                    _tracker.Updated.Add(ptoken);
                }

            }

            //Now for OR-Implicit we need to disable the other workitems
            //We need to determine that the workitem transition's arc is an or split one

            //Get all arcs related to the transition which fired
            var inward_arcs = from f in finishedWorkItem.Transition.Arcs
                              where f.ArcDirections == "IN"
                              select f;

            //If we had records then call the cancel
            List<WFArc> pinward_arcs = inward_arcs.ToList();

            if (pinward_arcs.Count > 0)
                if (pinward_arcs[0].ArcType == "OR-SPLIT-I")
                    this.CancelSplit(pinward_arcs[0], finishedWorkItem.Transition);


            //Let's find the arc which leave this transition and go to other places
            //Get all arcs leave this transition
            var outward_arcs = from g in finishedWorkItem.Transition.Arcs
                               where g.ArcDirections == "OUT"
                               select g;

            //If we had records then call the cancel
            List<WFArc> poutward_arcs = outward_arcs.ToList();

            if (poutward_arcs.Count == 0)
            {
                throw new Exception("There are no outbound places for transition " + finishedWorkItem.Transition.Id.ToString());
            }

            string arc_type = null;
            // check that all linked places have the same arc_type
            foreach (var arc in poutward_arcs)
            {
                if (arc_type == null)
                    arc_type = arc.ArcType;
                else
                {
                    if (arc_type != arc.ArcType)
                        throw new Exception("Outward arcs do not have the same ARC_TYP for transition" + finishedWorkItem.Transition.Id.ToString());
                }
            }

            //process arc by creating token according to the type of the arc
            switch (arc_type)
            {
                case "SEQ":
                    if (poutward_arcs.Count > 1)
                    {
                        throw new Exception("Cannot have more than one outward arc with ARC_TYPE=Sequential for transition" + finishedWorkItem.Transition.Id.ToString());
                    }
                    CreateNewToken(poutward_arcs[0].Place);
                    break;

                case "AND-SPLIT":
                    if (poutward_arcs.Count <= 1)
                    {
                        throw new Exception("Must have more than one outward arc with ARC_TYPE='AND-split' for traqnsition" + finishedWorkItem.Transition.Id.ToString());
                    }
                    foreach (var arc in poutward_arcs)
                    {
                        CreateNewToken(arc.Place);

                    } // foreach
                    break;
                case "OR-SPLIT-E":
                    if (poutward_arcs.Count <= 1)
                    {
                        throw new Exception("Must have more than one outward arc with ARC_TYPE='AND-split' for traqnsition" + finishedWorkItem.Transition.Id.ToString());
                    }

                    foreach (var arc in poutward_arcs)
                    {
                        //Process GUARD HERE
                        CreateNewToken(arc.Place);

                    } // foreach
                    break;
                case "OR-JOIN":
                    if (poutward_arcs.Count > 1)
                    {
                        throw new Exception("Cannot have more than one outward arc with ARC_TYPE=OR-JOIN for transition" + finishedWorkItem.Transition.Id.ToString());
                    }
                    CreateNewToken(poutward_arcs[0].Place);
                    break;

                default:
                    // "Invalid PLACE_TYPE on outward arc"
                    throw new Exception("Invalid place type");
            }


        }

        private void CancelSplit(WFArc arc, WFTransition transition)
        {

            // find out if the place on the current inward arc has other inward arcs
            var inward_arcs = from a in arc.Place.Arcs
                              where a.PlaceId == arc.Place.Id && a.ArcDirections == "IN" && a.TransitionId != transition.Id
                              select a;

            List<WFArc> pinward_arcs = inward_arcs.ToList();
            if (pinward_arcs.Count == 0)
            {
                throw new Exception("Error in current workflow setup. No enough arcs of type OR-split (Implicit) at place " + _workflow.Places.Single(s => s.Id == arc.Place.Id).Name);
            }
            else
            {
                string[] workitemStatus = new string[] { "EN", "IP" };
                //Let's get the current workitem
                foreach (var parc in pinward_arcs)
                {
                    if (parc.ArcType != arc.ArcType)
                        throw new Exception("Inward arcs do not have the same ARC_TYPE  from Place " + _workflow.Places.Single(s => s.Id == arc.Place.Id).Name);

                    //Get thee workitem associated with this arc through the transitions
                    var pworkitem = _case.WorkItems.Where(b => b.Transition.Id == parc.Transition.Id && workitemStatus.Contains(b.WorkitemStatus)).SingleOrDefault();


                    if (pworkitem == null)
                        throw new Exception("Expected workitem not found");
                    else
                        if (pworkitem.WorkitemStatus != "CA")
                        {
                            //Mark this workitem as cancelled
                            pworkitem.WorkitemStatus = "CA";
                            pworkitem.CancelledDate = DateTime.Now;
                            _tracker.Updated.Add(pworkitem);
                            //                $workitem_array['user_id']         = $_SESSION['logon_user_id'];
                            //We may need to set this up as an update item or new item


                        }
                }

            }

        }

        public static List<WFPlace> GetPlacesFromWorkflow(WFWorkflow wf, int? currentPlaceId = null)
        {
            List<WFPlace> _places = new List<WFPlace>();
            WFPlace currentPlace = null;
            if (currentPlaceId != null)
                currentPlace = wf.Places.Where(s => s.Id == currentPlaceId).Single();
            else
                currentPlace = wf.Places.Where(s => s.PlaceType == 1).Single(); //Would throw error if no starting place was defined

            GetPlacesFromPlace(currentPlace, _places);

            return _places;

            //Foreach arc get the transitions also leaving and for each transiti

        }

        private static void GetPlacesFromPlace(WFPlace currentPlace, List<WFPlace> _place)
        {

            //Get arcs leaving this place
            List<WFArc> arcsfromCurrentPlace = currentPlace.Arcs.Where(s => s.ArcDirections == "IN").ToList();

            //Continue factor
            if (arcsfromCurrentPlace.Count() == 1)
            {
                //Get the next place
                WFArc arcsfromCurrentTransition = arcsfromCurrentPlace[0].Transition.Arcs.Where(s => s.ArcDirections == "OUT").Single();
                if (arcsfromCurrentTransition.Place.PlaceType == 9)
                    return;
                else
                    _place.Add(arcsfromCurrentTransition.Place);


            }
            else if (arcsfromCurrentPlace.Count() == 0)
            {
                return;
            }
            else
            {

                //Stop factors
                foreach (var arc in arcsfromCurrentPlace)
                {
                    WFArc arcsfromCurrentTransition = arc.Transition.Arcs.Where(s => s.ArcDirections == "OUT").Single();
                    if (arcsfromCurrentTransition.Place.PlaceType == 9)
                        continue;
                    else
                        _place.Add(arcsfromCurrentTransition.Place);
                }
            }
        }

        public static List<WFTransition> GetTransitionsFromWorkflow(WFWorkflow wf, int? currentPlaceId = null)
        {
            List<WFTransition> _transitions = new List<WFTransition>();
            //Load workflow and read the starting place Id and if not place is define then start from the starting point
            WFPlace currentPlace = null;
            if (currentPlaceId != null)
                currentPlace = wf.Places.Where(s => s.Id == currentPlaceId).Single();
            else
                currentPlace = wf.Places.Where(s => s.PlaceType == 1).Single(); //Would throw error if no starting place was defined
            GetTransitionsFromPlace(currentPlace, _transitions);

            return _transitions;

        }

        private static void GetTransitionsFromPlace(WFPlace currentPlace, List<WFTransition> _transitions)
        {

            //Get arcs leaving this place
            List<WFArc> arcsfromCurrentPlace = currentPlace.Arcs.Where(s => s.ArcDirections == "IN").ToList();

            //Continue factor
            if (arcsfromCurrentPlace.Count() == 1 && arcsfromCurrentPlace[0].Transition.TaskTrigger == "AUTO")
            {
                //Get the next place
                WFArc arcsfromCurrentTransition = arcsfromCurrentPlace[0].Transition.Arcs.Where(s => s.ArcDirections == "OUT").Single();
                if (arcsfromCurrentTransition.Place.PlaceType == 9)
                    return;
                else
                    GetTransitionsFromPlace(arcsfromCurrentTransition.Place, _transitions);


            }
            else if (arcsfromCurrentPlace.Count() == 0)
            {
                return;
            }
            else
            {

                //Stop factors
                foreach (var arc in arcsfromCurrentPlace)
                {
                    _transitions.Add(arc.Transition);
                }
            }
        }



    }
}
