using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CRL.Repository.EF.All;

namespace CRL.Respositoy.EF.All.DataContextStorage
{
    public class HttpDataContextStorageContainer : IDataContextStorageContainer
    {
        private string _dataContextKey = "DataContext";

        public CBLContext GetDataContext()
        {
            CBLContext objectContext = null;
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                objectContext = (CBLContext)HttpContext.Current.Items[_dataContextKey];

            return objectContext;
        }

        public void Store(CBLContext CBLContext)
        {
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                HttpContext.Current.Items[_dataContextKey] = CBLContext;
            else
                HttpContext.Current.Items.Add(_dataContextKey, CBLContext);
        }

    }
}
