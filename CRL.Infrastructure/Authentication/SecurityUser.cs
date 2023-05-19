using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Authentication
{
    public class SecurityUser : IPrincipal, IIdentity
    {
        private string _roles;
        private string _name;
        private int _id;
        private string _email;
        private string _fullname;
        private int?   _institutionId ;
        private int? _institutionUnitId;
        private int _membershipId;
        private bool _isOwnerUser=false;
        private bool _requiresPasswordChange=false;
        private string _clientCode;
        private bool _IsPaypointUser;
        private int _AccountType;
        private short _MajorRoleIsSecuredPartyOrAgent;
        
        //Get the InstitutionId, MembershipId, InstitutionUnitId etc.

        public IIdentity Identity
        {
            get { return this; }
        }

        public bool IsInRole(string role)
        {
            string[] allroles = _roles.Split('|');
            return allroles.Contains(role.TrimEnd ('|'));
            //return Array.BinarySearch(allroles, role) >= 0 ? true : false;
        }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return _name; }
        }
        public bool IsOwnerUser
        {
            get { return _isOwnerUser; }
        }
        public bool IsPaypointUser
        {
            get { return _IsPaypointUser ; }
        }
        public int AccountType
        {
            get { return _AccountType ; }
        }

        public bool IsClientAdministrator()
        {
            return IsInRole("Administrator (Client)");
        }
        public bool IsClientUnitAdministrator()
        {
            return IsInRole("Unit Administrator (Client)");
        }
        public bool IsOwnerAdministrator()
        {
            return IsInRole("Administrator (Owner)");
        }      
        public bool IsOwnerAdminRegistrySupport()
        {
            return IsInAnyRoles("Administrator (Owner)", "Registrar","Support");
        }
        public bool IsOwnerRegistry()
        {
            return IsInRole("Registrar");
        }
        public bool IsOwnerUnitAdministrator()
        {
            return IsInRole("Unit Administrator (Owner)");
        }
        public bool IsAdministrator()
        {
            return IsInAnyRoles("Administrator (Owner)", "Administrator (Client)");
        }
        public bool IsFinanceOfficer()
        {
            return IsInAnyRoles("CRL Finance Officer", "Finance Officer");
        }
        public bool IsClientFinanceOfficer()
        {
            return IsInRole ( "Finance Officer");
        }
        public bool IsOwnerFinanceOfficer()
        {
            return IsInRole("CRL Finance Officer");
        }
        public bool IsAudit() 
        {
            return IsInRole("Audit");
        }
        public bool IsSupport()
        {
            return IsInAnyRoles("Support");
        }
        public bool IsUnitAdministrator()
        {
            return IsInAnyRoles("Unit Administrator (Owner)", "Unit Administrator (Client)");
        }
        public bool IsNonIndividualAdministrator()
        {
            return IsInAnyRoles("Administrator (Owner)", "Administrator (Client)") && !IsIndividualUser ;
        }
        public bool IsIndividualUser
        {
            get
            {
                if (_institutionId == null && _membershipId >0)
                    return true;
                else
                    return false;
            }
        }

        public bool RequiresPasswordChange
        {
            get { return _requiresPasswordChange; }
            set { _requiresPasswordChange = value; }
        }

        public string Roles
        {
            get { return _roles; }
        }
        public int Id { get { return _id ; } }
        public string Email { get { return _email ; } }
        public string FullName { get { return _fullname; } }

        public int? InstitutionId  { get { return _institutionId ; } }
        public int?  InstitutionUnitId { get { return _institutionUnitId ; } }

        public string ClientCode { get { return _clientCode ; } }
        public short MajorRoleIsSecuredPartyOrAgent { get { return _MajorRoleIsSecuredPartyOrAgent; } }
        public int MembershipId { get { return _membershipId ; } }
        // Checks whether a principal is in all of the specified set of 

        public bool IsInAllRoles(params string[] roles)
        {
            string[] allroles = _roles.Split('|');
            foreach (string searchrole in roles)
            {
                if (!allroles.Contains(searchrole.TrimEnd ('|')))
                    return false;
            }
            return true;
        }
        // Checks whether a principal is in any of the specified set of 

        public bool IsInAnyRoles(params string[] roles)
        {
            string[] allroles = _roles.Split('|');
            foreach (string searchrole in roles)
            {
                if (allroles.Contains(searchrole.TrimEnd ('|')))
                    return true;
            }
            return false;
        }

        public SecurityUser(int _ID, string _Name, string _FullName, string _Email, int _MembershipId, int? _InstitutionId, int? _InstitutionUnitId, string _Roles, bool _IsOwnerUser, string ClientCode, short MajorRoleIsSecuredPartyOrAgent, bool IsPaypointUser, int AccountType)
        {
            _id = _ID;
            _fullname  = _FullName;
            _name = _Name;
            _email = _Email;
            _roles = _Roles;
            _institutionId = _InstitutionId;
            _institutionUnitId = _InstitutionUnitId;
            _membershipId = _MembershipId;
            _isOwnerUser = _IsOwnerUser;
            _MajorRoleIsSecuredPartyOrAgent = MajorRoleIsSecuredPartyOrAgent;
            ClientCode = _clientCode;
            _IsPaypointUser = IsPaypointUser;
            _AccountType = AccountType;

        }



     



    }
}
