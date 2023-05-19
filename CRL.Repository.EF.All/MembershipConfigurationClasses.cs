using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema ;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.ModelViews;

using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using CRL.Model.Memberships;

namespace CRL.Repository.EF.All
{
    public class AddressInfoConfiguration
    : ComplexTypeConfiguration<AddressInfo>
    {
        public AddressInfoConfiguration()
        {
            Property(t => t.Address).HasMaxLength(255);
            Property(t => t.City).HasMaxLength(70);
            Property(t => t.Email).HasMaxLength(254);
            Property(t => t.Phone).HasMaxLength(50);



        }
    }

    public class PersonConfiguration
      : EntityTypeConfiguration<Person>
    {
        public PersonConfiguration()
        {
            Property(t => t.FirstName).HasMaxLength(50);
            Property(t => t.MiddleName).HasMaxLength(50);
            Property(t => t.Surname).HasMaxLength(50);
            Property(t => t.Title).HasMaxLength(10);
            Property(t => t.Gender).HasMaxLength(6);
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }
    }

    public class UserConfiguration
     : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable("Users");
            Property(t => t.Username).HasMaxLength(50);
            Property(t => t.Password).HasMaxLength(100);
            Property(t => t.PasswordSalt).HasMaxLength(100);
            HasMany(c => c.Roles).WithMany(p => p.Users).
             Map(m =>
             {
                 m.MapLeftKey("UserId");
                 m.MapRightKey("RoleId");
                 m.ToTable("UserRoles");
             });
        }
    }

    public class InstitutionConfiguration
     : EntityTypeConfiguration<Institution>
    {
        public InstitutionConfiguration()
        {
            Property(t => t.CompanyNo).HasMaxLength(50);
            Property(t => t.Name).HasMaxLength(150);

        }
    }

    public class MembershipConfiguration
    : EntityTypeConfiguration<Membership>
    {
        public MembershipConfiguration()
        {

            Property(t => t.ClientCode).HasMaxLength(17).IsFixedLength().
                HasColumnAnnotation("IX_ClientCode", new IndexAnnotation(new IndexAttribute("IX_ClientCode") { IsUnique = true })); 
            Property(t => t.AccountNumber).HasMaxLength(50);
            HasRequired(p => p.MembershipType).WithMany().HasForeignKey(p => p.MembershipTypeId);


        }

    }


    public class PasswordResetRequestConfiguration
    : EntityTypeConfiguration<PasswordResetRequest>
    {
        public PasswordResetRequestConfiguration()
        {

            Property(t => t.RequestCode).HasMaxLength(17).IsFixedLength();

        }
    }


}
