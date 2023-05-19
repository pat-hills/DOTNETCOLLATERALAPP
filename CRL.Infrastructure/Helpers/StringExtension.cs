using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Helpers
{
    public static class StringExtension
    {
        public static string TrimNull(this String s)
        {
            if (String.IsNullOrWhiteSpace(s))
              return null;
            else
               return s.Trim();
        }
    }
}
