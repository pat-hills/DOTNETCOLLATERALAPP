using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;
using CRL.Model.ModelViews;

using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.WorkflowEngine
{
    public class UserWorkflowAssignmentService
    {
        private IUserRepository _userRepository;
        IWFCaseRepository _caseRepository;
        IWFWorkflowRepository _workflowRepository;
        public List<User> Users { get; private set; }
        public List<string> UserMails { get; private set; }
        public int useThisMembershipId { get; set; }
        //This class takes a fully loaded clase and gets users
        //This class takes a place and loads user's, it would be preferrred to use case
        //This class will check if a user is qualified to be in a place
        //Will load all valid user
        public UserWorkflowAssignmentService()
        {

        }
        public UserWorkflowAssignmentService(IUserRepository userRepository, WFCase Case, SecurityUser user, WFTaskType? taskType = null, bool GenerateAssignedUsers = false, int? LimitToMembershipId = null)
        {
            _userRepository = userRepository;
            UserMails = new List<string>();
            var token = Case.Tokens.Where(s => s.TokenStatus == "FREE").SingleOrDefault();
            if (token != null)
            {
                List<WFPlace> _places = new List<WFPlace>();
                _places.Add(token.Place);
                Users = this.Process(userRepository, _places, user, taskType, LimitToMembershipId, Case);
                if (GenerateAssignedUsers)
                {
                    foreach (var usr in Users)
                    {
                        token.AssignedUsers.Add(usr);
                    }
                }
            }
            else
            {
                Users = new List<User>();
            }



            //Get the tokens

        }

        public UserWorkflowAssignmentService(IUserRepository userRepository, SecurityUser user, List<WFToken> tokens)
        {
            _userRepository = userRepository;
            UserMails = new List<string>();
            foreach (var token in tokens)
            {
                List<WFPlace> _places = new List<WFPlace>();
                _places.Add(token.Place);
                Users = this.Process(userRepository, _places, user, token.Case.TaskType, token.Case.LimitedToOtherMembershipId, token.Case);

                foreach (var usr in Users)
                {
                    if (usr.Id == user.Id)
                        token.AssignedUsers.Add(usr);
                }



            }



            //Get the tokens

        }
        public void InitialiseForGetCurrentUsersFromCase(IUserRepository userRepository, IWFCaseRepository caseRepository, IWFWorkflowRepository workflowRepository)
        {
            _userRepository = userRepository;
            _caseRepository = caseRepository;
            _workflowRepository = workflowRepository;
        }

        public List<User> GetCurrentUsersFromCase(int CaseId, SecurityUser user, WFTaskType? taskType = null, bool GenerateAssignedUsers = false, int? LimitToMembershipId = null)
        {
            WFCase _case = _caseRepository.FindBy(CaseId);
            WFToken _token = _case.Tokens.Where(tk => tk.IsActive == true && tk.IsDeleted == false && tk.TokenStatus == "FREE").FirstOrDefault();
            WFWorkflow wf = _workflowRepository.GetWFWorkflowById(_case.WorkflowId);
            // List<WFPlace> _places = WorkflowManager.GetPlacesFromWorkflow(wf, _token.PlaceId);                

            UserMails = new List<string>();
            var token = _case.Tokens.Where(s => s.TokenStatus == "FREE").SingleOrDefault();
            if (token != null)
            {
                List<WFPlace> _places = new List<WFPlace>();
                _places.Add(token.Place);
                Users = this.Process(_userRepository, _places, user, taskType, LimitToMembershipId);
                if (GenerateAssignedUsers)
                {
                    foreach (var usr in Users)
                    {
                        token.AssignedUsers.Add(usr);
                    }
                }
            }
            else
            {
                Users = new List<User>();
            }



            //Get the tokens

            return Users;

        }
        public UserWorkflowAssignmentService(IUserRepository userRepository, List<WFPlace> _places, SecurityUser user, WFTaskType? taskType = null, int? LimitToMembershipId = null)
        {
            _userRepository = userRepository;
            UserMails = new List<string>();
            Users = this.Process(userRepository, _places, user, taskType, LimitToMembershipId);
        }

        /// <summary>
        /// Pass the newly created case and check for the open tokens.Get the places and read the rules and extract the legitimate users
        /// </summary>
        /// <param name="Case"></param>
        public List<User> Process(IUserRepository userRepository, List<WFPlace> _places, SecurityUser user, WFTaskType? taskType = null, int? LimitToMembershipId = null, WFCase _case = null)
        {
            List<User> lstUsers = new List<User>();
            IQueryable<User> query = _userRepository.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.InBuiltUser == false);
            //Does this place has a rule settings and we also need the taskID
            foreach (var place in _places)
            {

                var assignmentConfiguration = place.WorkflowPlaceAssignmentConfigurations.Where(s => s.TaskId == taskType).SingleOrDefault();

                //If there are roles then
                if (assignmentConfiguration != null)
                {

                    
                    if (assignmentConfiguration.RoleLimitRules.Count > 0)
                    {

                        //Check if there are any roles limit
                        Roles[] MyInstitutionRoleIds_LimitUnit = assignmentConfiguration.RoleLimitRules.Where(d => d.LimitToInstitution == false && d.AllUnitsOrLimitToUnit ==1).Select(s => s.RoleId).ToArray(); //Select roles
                        Roles[] MyInstitutionRoleIds = assignmentConfiguration.RoleLimitRules.Where(d => d.LimitToInstitution == false && d.AllUnitsOrLimitToUnit ==0).Select(s => s.RoleId).ToArray(); //Select roles
                        Roles[] SelectedInstitutionRoleIds = assignmentConfiguration.RoleLimitRules.Where(d => d.LimitToInstitution == true).Select(s => s.RoleId).ToArray(); //Select roles for ins
                  
                        bool Has_MembershipRoles_Unit = MyInstitutionRoleIds_LimitUnit.Count() > 0;
                        bool Has_MembershipRoles_NoUnit = MyInstitutionRoleIds.Count() > 0;
                        bool Has_InstitutionRoles = SelectedInstitutionRoleIds.Count() > 0;

                        query = query.Where(s => (Has_MembershipRoles_NoUnit && s.Roles.Any(x => MyInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == user.MembershipId)
                            || (Has_MembershipRoles_Unit && s.Roles.Any(x => MyInstitutionRoleIds_LimitUnit.Contains(x.Id)) && (s.InstitutionUnitId == user.InstitutionUnitId || s.InstitutionUnitId ==null) && s.MembershipId == user.MembershipId)
                            || (Has_InstitutionRoles && s.Roles.Any(x => SelectedInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == LimitToMembershipId));


                        //if (MyInstitutionRoleIds.Count() > 0 && SelectedInstitutionRoleIds.Count() == 0)
                        //{
                        //    query = query.Where(s => s.Roles.Any(x => MyInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == user.MembershipId);
                        //}
                        //else if (MyInstitutionRoleIds.Count() == 0 && SelectedInstitutionRoleIds.Count() > 0)
                        //{
                        //    query = query.Where(s => s.Roles.Any(x => SelectedInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == LimitToMembershipId);
                        //}
                        //else if (MyInstitutionRoleIds.Count() > 0 && SelectedInstitutionRoleIds.Count() > 0)
                        //{
                        //    query = query.Where(s => s.Roles.Any(x => MyInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == user.MembershipId ||
                        //       s.Roles.Any(x => SelectedInstitutionRoleIds.Contains(x.Id)) && s.MembershipId == LimitToMembershipId);
                            
                        //}

                        var users = _userRepository.FindBy(query).ToList();
                        //We also need to know if we need to 
                        foreach (var usr in users)
                        {
                            if (!lstUsers.Contains(usr))
                            {
                                lstUsers.Add(usr);
                                UserMails.Add(usr.Address.Email);

                            }

                        }
                    }

                    if (_case != null)
                    {
                        if (assignmentConfiguration.ReAssignToCaseCreator)
                        {
                            var usr = _userRepository.FindBy(_case.CreatedBy);
                            if (!lstUsers.Contains(usr))
                            {
                                lstUsers.Add(usr);
                            }
                        }

                        if (assignmentConfiguration.ReAssignToPreviousSender)
                        {
                            WFWorkItem w = _case.WorkItems.Where(s => s.WorkitemStatus == "FI").OrderByDescending(s => s.FinishedDate).FirstOrDefault();
                            if (w != null)
                            {
                                var usr = _userRepository.FindBy((int)w.UpdatedBy);
                                if (!lstUsers.Contains(usr))
                                {
                                    lstUsers.Add(usr);
                                }
                            }
                        }
                    }




                }




            }
            //Read the rule for that token
            return lstUsers;
            //Generate the users

        }
    }
}
