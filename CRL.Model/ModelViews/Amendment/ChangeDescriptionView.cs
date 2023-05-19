using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Amendment
{
    public class ChangeDescriptionView
    {
        public int UpdateId { get; set; }
        [Display(Name = "Update description")]
        public string Operation { get; set; }
        [Display(Name = "Item")]
        public string Object { get; set; }
        [Display(Name = "Old Information")]
        public string BeforeUpdate { get; set; }
        [Display(Name = "New Information")]
        public string AfterUpdate { get; set; }
    }
}
