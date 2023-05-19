using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service
{
    public class NoValidCBLAdministratorEmailException:ApplicationException
    {
        public NoValidCBLAdministratorEmailException() : base("An exception occurred when trying to get any CBL Administrator email address!") { }
        public NoValidCBLAdministratorEmailException(string message) : base("An exception occurred when trying to get any CBL Administrator email address! " + message) { }

    }
}
