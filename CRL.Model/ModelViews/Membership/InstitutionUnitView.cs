using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Model.ModelViews.Memberships
{
    public class InstitutionUnitView
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid name provided")]
        public string Name { get; set; }
        public string Description { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public int InstitutionId { get; set; }        
        public ICollection<UserView> UsersView { get; set; }
    }
}
