using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Repository.EF.All;

namespace CRL.Respositoy.EF.All.DataContextStorage
{
    public interface IDataContextStorageContainer
    {
        CBLContext GetDataContext();
        void Store(CBLContext CBLContext);
    }
}
