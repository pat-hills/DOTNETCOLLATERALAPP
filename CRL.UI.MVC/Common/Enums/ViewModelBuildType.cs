using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Common.Enums
{
    /// <summary>
    /// This enumeration is used when building view models from service response.  For value New is used for an initial building of a view model
    /// .  The value Rebuild means the view model has data already but needs some other data mostly lookups for the view again.
    /// </summary>
    public enum ViewModelBuildType
    {
        New=1, Rebuild=2
    }
}