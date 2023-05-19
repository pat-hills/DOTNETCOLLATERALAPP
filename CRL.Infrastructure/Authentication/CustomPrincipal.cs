using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Authentication
{
    public class CustomPrincipal : System.Security.Principal.IPrincipal
    {

        private CustomIdentity _identity;
        private string[] _roles;

        public CustomPrincipal(CustomIdentity identity, string[] roles)
        {

            _identity = identity;
            _roles = new string[roles.Length];
            roles.CopyTo(_roles, 0);
            Array.Sort(_roles);

        }

        public System.Security.Principal.IIdentity Identity
        {

            get { return _identity; }

        }

     
        public string Username
        {
            get
            {
                return _identity.Name;
            }
        }

      

     


        // IPrincipal Implementation
        public bool IsInRole(string role)
        {
            return Array.BinarySearch(_roles, role) >= 0 ? true : false;
        }
        // Checks whether a principal is in all of the specified set of 

        public bool IsInAllRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.BinarySearch(_roles, searchrole) < 0)
                    return false;
            }
            return true;
        }
        // Checks whether a principal is in any of the specified set of 

        public bool IsInAnyRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.BinarySearch(_roles, searchrole) > 0)
                    return true;
            }
            return false;
        }

    }
}
