using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Service.Common
{
    [Serializable]
    public class ViewBase
    {
        public RecordState RecordState { get; set; }
    }
}
