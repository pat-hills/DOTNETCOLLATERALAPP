using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Infrastructure.Messaging;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// Represents people who have a membership account in the system
    /// </summary>
    [Serializable]
    public class User : Person
    {
        public User()
        {
            this.Roles = new HashSet<Role>();
            this.RoleGroups = new HashSet<RoleGroup>();
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        //public bool IsApproved { get; set; }
        public Nullable<DateTime> LastActivityDate { get; set; }
        public Nullable<DateTime> LastLoginDate { get; set; }
        public Nullable<DateTime> LastPasswordChangeDate { get; set; }
        public bool IsOnLine { get; set; }
        public bool IsLockedOut { get; set; }
        public Nullable<DateTime> LastLockedOutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public bool isPayPointUser { get; set; }
        public bool ResetPasswordNextLogin { get; set; }
        public string ResetPasswordNextLoginCode { get; set; }
        public bool BuildUserSettingOnLogin { get; set; }
        //Relationships
        public int? MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<RoleGroup> RoleGroups { get; set; }
        public bool InBuiltUser { get; set; }

        public static ResponseBase ValidateUserUniqueness(int UserId, string Email, IUserRepository _userRepository)
        {
            ResponseBase response = new ResponseBase();
            if (_userRepository.GetNonDeletedUserByEmail(Email).Where (s=>s.Id != UserId).Count() > 0 )
            {
                response.MessageInfo = new MessageInfo();
                response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                response.MessageInfo.Message = "The email address of the security user or individual user is already in used by another user account";//
                return response;
            }

            response.GenerateDefaultSuccessMessage();
            return response;
        }
        public static ResponseBase ValidateUserUniqueness(string LoginId, string Email, IUserRepository _userRepository)
        {
            ResponseBase response = new ResponseBase();

            if (_userRepository.GetNonDeletedUserByLoginId(LoginId).Count() > 0)
            {
                response.MessageInfo = new MessageInfo();
                response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                response.MessageInfo.Message = "User Login Id is already taken please specify another login ID";
                return response;
            }

            if (_userRepository.GetNonDeletedUserByEmail  (Email).Count() > 0)
                {
                    response.MessageInfo = new MessageInfo();
                    response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                    response.MessageInfo.Message = "The email address of the security user or individual user is already in used by another user account";//
                    return response;
                }

            response.GenerateDefaultSuccessMessage();
            return response;
        }
    


    }
}
