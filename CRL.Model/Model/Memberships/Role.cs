using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.ModelViews.Enums;

namespace CRL.Model.Memberships
{
    public enum Roles {AdminOwner=1,AdminClient=2,AdminUnitOwner=3, AdminUnitClient=4,
        FSAuthorizer=5,FSOfficer=6,AmendOfficer=8,AmendAuthorizer=9,
        UpdateAuthorizer=10,
        UpdateOfficer=11,
        DischargeAuthorizer=12,
        DischargeOfficer=13,
        SubordinateAuthorizer=14,
        SubordinateOfficer=15, 
        AssigmentAuthorizer=16,
        AssignmentOfficer=17,
       
        FinanceOfficer=18,
        ClientOfficer=19,
        ClientAuthorizer=22,
        CRLFinanceOfficer=23,
        Audit=25,Registrar=26,
        Support=27,Search=28}
    /// <summary>
    /// Role the user
    /// </summary>
    [Serializable]
    public class Role : AuditedEntityBaseModel<Roles>, IAggregateRoot
    {
        public Role()
        {
            this.Users = new HashSet<User>();
        }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Label { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }        
        public bool PredefinedInSystem { get; set; }
      

        //Relationships
        public int RoleCategoryId { get; set; }
        public virtual RoleCategory RoleCategory { get; set; }
        public int? RoleGroupId { get; set; }
        /// <summary>
        /// Indicates which membership type (Developer, Client or Owner) this role is limited to.  Null value indicates that it is for all
        /// </summary>
        public Nullable<MembershipCategory> MembershipCategoryId { get; set; }  //Limit role to enterprise category
        /// <summary>
        /// Indicate if this role is limited to Individual or Financial Institution.  If null then there is no limit
        /// </summary>
        public Nullable<short> LimitToIndividualOrInstitution { get; set; }  //On means to Indivdual Off Means Institution Null meas all
        /// <summary>
        /// Indicate wether this role is limited to a unit or enterprise.  If null then there is not limit
        /// </summary>
        public Nullable<short> LimitToUnitOrEnterprise { get; set; }  //Limit role ti Individual
        public virtual ICollection<User> Users { get; set; }
        public virtual LKMembershipCategory MembershipType { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
