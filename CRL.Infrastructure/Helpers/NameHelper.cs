using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Helpers
{
    public static class NameHelper
    {
        public static string GetFullName(string FirstName, string MiddleName, string Surname)
        {
            if (MiddleName == null)
            {
                MiddleName = "";
            }
            return ((FirstName.TrimEnd() + " " + MiddleName.TrimStart()).Trim() + " " + Surname.TrimStart()).Trim();
        }
    }
}
