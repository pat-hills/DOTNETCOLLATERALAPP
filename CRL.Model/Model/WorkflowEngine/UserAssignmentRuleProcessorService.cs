using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Model.WorkflowEngine;

namespace CRL.Model.WorkflowEngine{
    //public  class UserAssignmentRuleProcessorService
    //{
      
    //    private IUserRepository _userRepository;
    //    private List<User> Users = new List<User>();
       

    //    public UserAssignmentRuleProcessorService(IUserRepository userRepository) //User refUser, IUserRepository userRepository)
    //    {    
    //        _userRepository=userRepository ;
    //        //Here we are going to read the rules and then 
          
    //    }



    //    public List<User> GenerateAssignedUsersFromTransitions(int _executingUser, List<WFTransition> _transitions, WFCase _case = null, int? membershipid = null)
    //    {
    //        User executingUser = _userRepository.FindBy(_executingUser);
    //        User caseUser = _case!=null?  _userRepository.FindBy(_case.CreatedBy):executingUser ;
    //        foreach (var wrk in _transitions)
    //        {
    //            //Get the associated transition assignments

    //            //For each transition assigments read the rules and for each rules get the users


    //            var transitionAssignments = wrk.TransitionAssignments; //*We need to filter to avoid waste of time

    //            foreach (var tA in transitionAssignments)
    //            {

    //                //Get the rules
    //                var rules = tA.UserAssignmentRules;  //UPDATE this to load only global and membership specific 
    //                foreach (var rule in rules)
    //                {
    //                    List<User> lstUsers = new List<User>();

    //                    if (tA.UseCaseCreaterMembershipOrExecutingUserMembership == 1)
    //                        lstUsers = GetUsersFromRule(rule, caseUser,_case, membershipid);
    //                    else
    //                        lstUsers = GetUsersFromRule(rule, executingUser,_case,membershipid);

    //                    foreach (var usr in lstUsers)
    //                    {

    //                        if (!Users.Contains(usr))
    //                        {
    //                            Users.Add(usr);
    //                        }
    //                    }

    //                }



    //            }


    //        }




    //        return Users;
    //    }

    //    public List<User> GenerateAssignedUsersForWorkItems(WFCase _case, int _executingUser,int? membershipid=null)
    //    {
           
    //        User executingUser = _userRepository.FindBy(_executingUser);
    //        User caseUser = _userRepository.FindBy(_case.CreatedBy);

    //        //Get the latest enabled workitems for us to determine the assign user mails
    //        var selectedCurrentWorkItems = _case.WorkItems.Where(s => s.WorkitemStatus == "EN");
    //        foreach (var wrk in selectedCurrentWorkItems)
    //        {
    //            //Get the associated transition assignments

    //            //For each transition assigments read the rules and for each rules get the users


    //            var transitionAssignments = wrk.Transition.TransitionAssignments; //*We need to filter to avoid waste of time
              
    //            foreach (var tA in transitionAssignments)
    //            {

    //                //Get the rules
    //                var rules = tA.UserAssignmentRules;  //UPDATE this to load only global and membership specific 
    //                foreach (var rule in rules)
    //                {
    //                    List<User> lstUsers = new List<User>();
                        
    //                    if (tA.UseCaseCreaterMembershipOrExecutingUserMembership == 1)
    //                        lstUsers = GetUsersFromRule(rule, caseUser, _case, membershipid);
    //                    else
    //                        lstUsers = GetUsersFromRule(rule, executingUser, _case, membershipid);

    //                    foreach (var usr in lstUsers)
    //                    {

    //                        if (!wrk.AssignedUsers.Contains(usr))
    //                        {
    //                            wrk.AssignedUsers.Add(usr);
    //                        }

    //                        if (!Users.Contains(usr))
    //                        {
    //                            Users.Add(usr);
    //                        }
    //                    }
                    
    //                }

                    

    //            }


    //        }




    //        return Users;


    //    }

    //    //public List<User> GetUsersFromRule(WFCase _case, int _executingUser) //ICollection<WFUserAssignmentConfigurationRule> UserAssignmentRules)
    //    //{
            
    //    //    User executingUser = _userRepository.FindBy(_executingUser);
    //    //    User caseUser = _userRepository.FindBy(_case.CreatedBy  );
    //    //    //Get the latest enabled workitems for us to determine the assign user mails
    //    //    var selectedCurrentWorkItems = _case.WorkItems.Where(s => s.WorkitemStatus == "EN");
    //    //    foreach (var wrk in selectedCurrentWorkItems)
    //    //    {
    //    //        //Get the associated transition assignments

    //    //        //For each transition assigments read the rules and for each rules get the users


    //    //        var  transitionAssignments = wrk.Transition.TransitionAssignments ; //*We need to filter to avoid waste of time
    //    //        foreach (var tA in transitionAssignments)
    //    //        {
    //    //            //Get the rules
    //    //            var rules = tA.UserAssignmentRules;  //UPDATE this to load only global and membership specific 
    //    //            foreach (var rule in rules)
    //    //            {
    //    //                if (tA.UseCaseCreaterMembershipOrExecutingUserMembership ==1)
    //    //                    GetUsersFromRule(_case, rule, caseUser);
    //    //                else
    //    //                   GetUsersFromRule(_case,rule, executingUser);
    //    //            }

    //    //        }
                

    //    //    }

           
          

    //    //    return Users;
    //    //}

    //    private    List<User>  GetUsersFromRule(WFUserAssignmentConfigurationRule rule, User _refUser,WFCase _case=null, int? LimitToSpecificMembershipId=null)
    //    {
    //        //Check rule if we have users included
    //        List<User> lstUsers = new List<User>();
    //        if (rule.LimitToUsers.Count > 0)
    //        {
    //            foreach (var usr in rule.LimitToUsers)
    //            {
    //                if (!lstUsers.Contains(usr))
    //                {
    //                    lstUsers.Add(usr);
    //                }
                    
    //            }
                
    //        }

    //        //Load roles for rule
    //        if (rule.LimitToRoles.Count > 0)
    //        {
    //             IQueryable<User> query = _userRepository.GetDbSet();
    //            List<int> rolesd= new List<int> ();
    //            int[] RoleIds = rule.LimitToRoles.Select(s => s.Id).ToArray();
    //            //Get array of roles id

    //            query = query.Where(s => s.Roles.Any(x => RoleIds.Contains(x.Id)));
               
               
               

    //            if (rule.LimitRoleUsersToRefUsrMembership  == true)
    //            {
    //                query = query.Where(s => s.MembershipId  == _refUser.MembershipId );
    //            }
    //            if (rule.LimitRoleUsersToRefUsrUnit == true)
    //            {
    //                if (_refUser.MembershipId  != null)
    //                {
    //                    query = query.Where(s => s.MembershipId  == _refUser.MembershipId  && s.InstitutionUnitId == _refUser.InstitutionUnitId);
    //                }
    //            }

    //            if (rule.LimitRoleToASpecificMembership == true)
    //            {
    //                query = query.Where(s => s.MembershipId == LimitToSpecificMembershipId);
    //            }
                
    //            foreach (var usr in _userRepository.FindBy(query).ToList())
    //            {
    //                if (!lstUsers.Contains(usr))
    //                {
    //                    lstUsers.Add(usr);
    //                }

    //            }
    //        }

          

    //        if (_case != null)
    //        {
    //            if (rule.AssignCaseCreator)
    //            {
    //                var usr = _userRepository.FindBy(_case.CreatedBy);
    //                if (!lstUsers.Contains(usr))
    //                {
    //                    lstUsers.Add(usr);
    //                }
    //            }

    //            if (rule.AssignPreviousSender)
    //            {
    //                WFWorkItem w = _case.WorkItems.Where(s => s.WorkitemStatus == "FI").OrderByDescending(s => s.FinishedDate).FirstOrDefault();
    //                if (w != null)
    //                {
    //                    var usr = _userRepository.FindBy((int)w.UpdatedBy);
    //                    if (!lstUsers.Contains(usr))
    //                    {
    //                        lstUsers.Add(usr);
    //                    }
    //                }
    //            }
    //        }

    //        return lstUsers;

        
    //    }

      
    //}
}
