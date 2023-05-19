using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Enums;
using CRL.UI.MVC.Common.Enums;

namespace CRL.UI.MVC.Common
{
    public abstract class BaseDetailViewModel
    {
        public BaseDetailViewModel()
        {
        }

        public BaseDetailViewModel(EditMode mode)
        {
            this.FormMode = mode;
        }

        public EditMode FormMode { get; set; }
        public string UniqueGuidForm { get; set; }
        public virtual void ChangeFormMode(EditMode formMode)
        {
            FormMode = formMode;
        }
    }
}