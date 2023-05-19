using CRL.Service.Views.Configuration;
using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel
{
    public class BVCUploadViewModel
    {
        public BVCUploadViewModel()
        {
            BankCodeGridView = new List<BankVerificationCodeView>();
        }

        public List<BankVerificationCodeView> BankCodeGridView { get; set; }
        public _BvcJqgridViewModel _BvcJqgridViewModel { get; set; }


        [Required]
        public HttpPostedFileBase CSVfile { get; set; }
        
    }


}