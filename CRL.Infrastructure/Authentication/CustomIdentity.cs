using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace CRL.Infrastructure.Authentication
{
    public class CustomIdentity : System.Security.Principal.IIdentity
    {

        private FormsAuthenticationTicket _ticket;

        public CustomIdentity(FormsAuthenticationTicket ticket)
        {

            _ticket = ticket;

        }
        public FormsAuthenticationTicket Ticket
        {

            get { return _ticket; }

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

            get { return _ticket.Name; }

        }
        
       
        public string Roles
        {

            get
            {

                string[] userDataPieces = _ticket.UserData.Split("|".ToCharArray());
                string roles = "";
                for (int i = 4; i < userDataPieces.Length; i++)
                {
                    roles = roles + userDataPieces[i] + "|";
                }
                return roles.TrimEnd("|".ToCharArray());

            }

        }




    }
}
