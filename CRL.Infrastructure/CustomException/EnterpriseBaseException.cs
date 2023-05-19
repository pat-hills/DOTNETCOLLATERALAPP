using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.CustomException
{
    //Use masing
    //111 - means show to developer, show to 
    //Show to only developer if level is one
    //Show error to CRG User if level is two
    //Show to client if level is three




    public enum EnterpriseBaseExceptionLevel
    {
        OnlyDeveloper, OnlyDeveloperAndOwner, All
    }
    public class EnterpriseBaseException: ApplicationException
    {
        public EnterpriseBaseException()
        : base() { }
    
    public EnterpriseBaseException(string message)
        : base(message) { }

    public EnterpriseBaseException(string message,EnterpriseBaseExceptionLevel viewLevel)
        : base(message) {this.ViewLevel = viewLevel; }
    
    public EnterpriseBaseException(string format, params object[] args)
        : base(string.Format(format, args)) { }
    
    public EnterpriseBaseException(string message, Exception innerException)
        : base(message, innerException) { }
    
    public EnterpriseBaseException(string message, Exception innerException,EnterpriseBaseExceptionLevel viewLevel)
        : base(message, innerException) {this.ViewLevel = viewLevel; }
    
    public EnterpriseBaseException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }

    protected EnterpriseBaseException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    public EnterpriseBaseExceptionLevel ViewLevel { get; set; }
    }
}
