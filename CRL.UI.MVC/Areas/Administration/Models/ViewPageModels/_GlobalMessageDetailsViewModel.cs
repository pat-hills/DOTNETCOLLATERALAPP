using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Helpers;
using CRL.UI.MVC.Common;
using System.ComponentModel.DataAnnotations;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class _GlobalMessageDetailsViewModel : BaseDetailViewModel
    {
        public _GlobalMessageDetailsViewModel()
        {
            MessageRolesList = new List<LookUpView>();
            SelectedMessageRolesList = new List<LookUpView>();
        }
      
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        public int MessageTypeId { get; set; }
        public bool IsLimitedToAdmin { get; set; }
        public bool IsLimitedToClientOrOwners { get; set; }
        public bool IsLimitedToInstitutionOrIndividual { get; set; }
        public int[] MessageRoles { get; set; }
        public ICollection<LookUpView> MessageRolesList { get; set; }
        public ICollection<LookUpView> SelectedMessageRolesList { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}