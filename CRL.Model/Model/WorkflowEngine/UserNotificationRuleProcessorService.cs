using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Model.WorkflowEngine;

namespace CRL.Model.WorkflowEngine
{
    //public class UserNotificationRuleProcessorService
    //{
    //       private IUserRepository _userRepository;
    //       List<string> Emails = new List<string>();


    //       public UserNotificationRuleProcessorService(IUserRepository userRepository) //User refUser, IUserRepository userRepository)
    //    {    
    //        _userRepository=userRepository ;
    //        //Here we are going to read the rules and then 
          
    //    }

    //    public List<string> GetEmailsFromRule(WFCase _case, int _executingUser, List<User> AssignedUsers) //ICollection<WFUserAssignmentConfigurationRule> UserAssignmentRules)
    //    {
           
    //        User executingUser = _userRepository.FindBy(_executingUser);
    //        User caseUser = _userRepository.FindBy(_case.CreatedBy);
    //        //Get the latest enabled workitems for us to determine the assign user mails
    //        var selectedCurrentWorkItems = _case.WorkItems.Where(s => s.WorkitemStatus == "EN");
    //        foreach (var wrk in selectedCurrentWorkItems)
    //        {
    //            //Get the associated transition assignments

    //            //For each transition assigments read the rules and for each rules get the users


    //            var  transitionAssignments = wrk.Transition.TransitionAssignments ;
    //            foreach (var tA in transitionAssignments)
    //            {
    //                //Get the rules
    //                var rules = tA.UserNotificationRules;  //UPDATE this to load only global and membership specific 
    //                foreach (var rule in rules)
    //                {
    //                    if (tA.UseCaseCreaterMembershipOrExecutingUserMembership ==1)
    //                        GetEmailsFromRule(_case, rule, caseUser, executingUser, AssignedUsers);
    //                    else
    //                        GetEmailsFromRule(_case, rule, executingUser, executingUser, AssignedUsers);
    //                }

    //            }
                

    //        }




    //        return Emails;
    //    }

    //    private  void GetEmailsFromRule(WFCase _case, WFWorkflowNotificationConfigurationRule rule, User _refUser, User _executingUser, List<User> AssignedUsers)
    //    {
    //        //Check rule if we have users included         
    //        if (rule.LimitToUsers.Count > 0)
    //        {
               
    //            foreach (var usr in rule.LimitToUsers)
    //            {
    //                if (!Emails.Contains(usr.Address.Email ))
    //                {
    //                    Emails.Add(usr.Address.Email);
    //                }

    //            }

                
    //        }

    //        //Load roles for rule
    //        if (rule.LimitToRoles.Count > 0)
    //        {

    //            IQueryable<User> query = _userRepository.GetDbSet();
    //            List<int> rolesId = new List<int>();
    //            //Get array of roles id
    //            foreach (var role in rule.LimitToRoles)
    //            {
    //                query = query.Where(s => s.Roles.Any(x => x.Id == role.Id));
    //            }
                
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

    //            foreach (var usr in _userRepository.FindBy(query).ToList())
    //            {
    //                if (!Emails.Contains(usr.Address.Email))
    //                {
    //                    Emails.Add(usr.Address.Email);
    //                }
                  

    //            }

              

                
    //        }

    //        if (rule.NotifyAssignedUsers )
    //        {
    //            foreach (var usr in AssignedUsers )
    //            {
    //                if (!Emails.Contains(usr.Address.Email))
    //                {
    //                    Emails.Add(usr.Address.Email);
    //                }

    //            }
               
    //        }

    //        if (rule.NotifySender )
    //        {
    //            if (!Emails.Contains(_executingUser.Address.Email))
    //            {
    //                Emails.Add(_executingUser.Address.Email);
    //            }             
                
    //        }



      
    //    }
    //}
}
