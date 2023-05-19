using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CRL.Repository.EF.All;
using CRL.Respositoy.EF.All.DataContextStorage;

namespace CRL.Respositoy.EF.All.DataContextStorage
{
    public class ThreadDataContextStorageContainer : IDataContextStorageContainer
    {
        private static readonly Hashtable _CBLContext = new Hashtable();

        public CBLContext GetDataContext()
        {
            CBLContext CBLContext = null;

            if (_CBLContext.Contains(GetThreadName()))
                CBLContext = (CBLContext)_CBLContext[GetThreadName()];

            return CBLContext;
        }

        public void Store(CBLContext CBLContext)
        {
            if (_CBLContext.Contains(GetThreadName()))
                _CBLContext[GetThreadName()] = CBLContext;
            else
                _CBLContext.Add(GetThreadName(), CBLContext);
        }

        private static string GetThreadName()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Kofi";
            return Thread.CurrentThread.Name;
        }
    }
}
