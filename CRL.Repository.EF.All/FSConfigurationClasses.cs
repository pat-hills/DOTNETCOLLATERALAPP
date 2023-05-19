using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.FS;
using CRL.Model.FS;

namespace CRL.Repository.EF.All
{
    public class FinancialStatementConfiguration
   : EntityTypeConfiguration<FinancialStatement>
    {
        public FinancialStatementConfiguration()
        {
            Property(t => t.RegistrationNo).HasMaxLength(17).IsFixedLength();
            Property(t => t.RequestNo).HasMaxLength(17).IsFixedLength();
            Property(t => t.FinancialStatementLastActivity).HasMaxLength(50);
            HasOptional(p => p.FinancialStatementLoanType)
              .WithMany()
              .HasForeignKey(p => p.FinancialStatementLoanTypeId);
           HasRequired(p => p.FinancialStatementTransactionType)
              .WithMany()
              .HasForeignKey(p => p.FinancialStatementTransactionTypeId);


            HasRequired(p => p.Membership)
              .WithMany()
              .HasForeignKey(p => p.MembershipId).WillCascadeOnDelete(false);

           HasRequired(p => p.MaximumAmountSecuredCurrency)
          .WithMany()
          .HasForeignKey(p => p.MaximumAmountSecuredCurrencyId);

           HasRequired(p => p.CollateralType)
         .WithMany()
         .HasForeignKey(p => p.CollateralTypeId);

           HasOptional(p => p.VerificationAttachment)
       .WithMany()
       .HasForeignKey(p => p.VerificationAttachmentId);

           HasMany(p => p.FileAttachments)
 .WithMany(p=>p.FinancialStatements);




        }
    }

    public class PersonIdentificationInfoConfiguration
    : ComplexTypeConfiguration<PersonIdentificationInfo>
    {
        public PersonIdentificationInfoConfiguration()
        {
            Property(t => t.FirstName).HasMaxLength(50);
            Property(t => t.MiddleName).HasMaxLength(200);
            Property(t => t.Surname).HasMaxLength(50);
            Property(t => t.CardNo ).HasMaxLength(25);
            Property(t => t.OtherDocumentDescription ).HasMaxLength(50);
         



        }
    }

    public class OtherPersonIdentificationInfoConfiguration
   : EntityTypeConfiguration<PersonIdentification>
    {
        public OtherPersonIdentificationInfoConfiguration()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.UniqueCode ).HasMaxLength(24).IsFixedLength();

            //PersonIdentificationCardType
            HasRequired(p => p.PersonIdentificationType).WithMany().HasForeignKey(p => p.PersonIdentificationTypeId);

            HasRequired(p => p.IndividualParticipant)
     .WithMany(p => p.OtherPersonIdentifications)
     .HasForeignKey(p => p.IndividualParticipantId);





        }
    }
    public class IndividualParticipantConfiguration
     : EntityTypeConfiguration<IndividualParticipant>
    {
        public IndividualParticipantConfiguration()
        {
            ToTable("IndividualParticipants");
            Property(t => t.Title).HasMaxLength(5);
            Property(t => t.Gender).HasMaxLength(6);
            Property(t => t.OtherDocumentDescription).HasMaxLength(50);

        }
    }

    public class InstitutionParticipantConfiguration
    : EntityTypeConfiguration<InstitutionParticipant>
    {
        public InstitutionParticipantConfiguration()
        {
            ToTable("InstitutionParticipants");
            Property(t => t.CompanyNo).HasMaxLength(50);
            Property(t => t.Name).HasMaxLength(150);
            Property(t => t.SearchableName).HasMaxLength(150);
            

        }
    }

    public class ParticipantConfiguration
  : EntityTypeConfiguration<Participant>
    {
        public ParticipantConfiguration()
        {

            Property(t => t.ParticipantNo ).HasMaxLength(24).IsFixedLength();

            //Participants Lookups
           HasRequired(p => p.ParticipantType)
         .WithMany()
         .HasForeignKey(p => p.ParticipantTypeId);

           HasRequired(p => p.ParticipationType)
          .WithMany()
          .HasForeignKey(p => p.ParticipationTypeId);

           HasMany(p => p.SectorOfOperationTypes)
            .WithMany(c => c.Participants);
        

        }
    }


    public class CollateralConfiguration
   : EntityTypeConfiguration<Collateral>
    {
        public CollateralConfiguration()
        {
            Property(t => t.CollateralNo).HasMaxLength(24).IsFixedLength();
            Property(t => t.Description ).HasMaxLength(255);
            Property(t => t.SerialNo ).HasMaxLength(100);
        }
    }


    public class IndividualSubordinatingPartyConfiguration
  : EntityTypeConfiguration<IndividualSubordinatingParty>
    {
        public IndividualSubordinatingPartyConfiguration()
        {
            ToTable("IndividualSubordinatingParties");
            Property(t => t.Title).HasMaxLength(5);
            Property(t => t.Gender).HasMaxLength(6);
            Property(t => t.OtherDocumentDescription).HasMaxLength(50);
        }
    }

    public class InstitutionSubordinatingPartyConfiguration
: EntityTypeConfiguration<InstitutionSubordinatingParty>
    {
        public InstitutionSubordinatingPartyConfiguration()
        {
            ToTable("InstitutionSubordinatingParties");
            Property(t => t.CompanyNo).HasMaxLength(50);
            Property(t => t.Name).HasMaxLength(150);
        }
    }

    public class FinancialStatementActivityConfiguration
 : EntityTypeConfiguration<FinancialStatementActivity>
    {
        public FinancialStatementActivityConfiguration()
        {
            Property(t => t.ActivityCode).HasMaxLength(17).IsFixedLength();
        }
    }

    public class ActivityAssignmentConfiguration
: EntityTypeConfiguration<ActivityAssignment>
    {
        public ActivityAssignmentConfiguration()
        {
            ToTable("ActivityAssignment");
            Property(t => t.Description ).HasMaxLength(255);
           HasRequired(p => p.AssignedMembershipFrom  )
              .WithMany()
              .HasForeignKey(p => p.AssignedMembershipFromId ).WillCascadeOnDelete (false);

        }
    }


    public class ActivitySubordinationConfiguration
: EntityTypeConfiguration<ActivitySubordination>
    {
        public ActivitySubordinationConfiguration()
        {
            ToTable("ActivitySubordination");
            Property(t => t.SubordinationComment ).HasMaxLength(255);

        }
    }


    public class ActivityUpdateConfiguration
: EntityTypeConfiguration<ActivityUpdate>
    {
        public ActivityUpdateConfiguration()
        {
            ToTable("ActivityUpdates");

        }
    }


}
