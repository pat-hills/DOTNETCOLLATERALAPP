using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Views.Workflow
{
    //public class AssignedToUser
    //{
    //    public int Id { get; set; }
    //    public string Fullname { get; set; }
    //    public string Username { get; set; }
    //    public string Description { get; set; }

    //}
    public class AssignNewTaskView
    {
        public ICollection<UserView> AssignedToUser { get; set; } //**Use a lighter one not user view
        //[Required(ErrorMessage="A comment for the workflow section of this page is required.")]
        [Display(Name="Please add any comment below")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

    }
}
