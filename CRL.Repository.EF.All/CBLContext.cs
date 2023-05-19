using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using CRL.Model.ModelViews;
using CRL.Model.FS;
using System.ComponentModel.DataAnnotations.Schema;

using CRL.Model.Search;
using CRL.Model.Common;
using CRL.Model;
using CRL.Model.WorkflowEngine;
using CRL.Model.Notification;
using CRL.Model.Payments;
using CRL.Model.Configuration;
using CRL.Model.Reporting;
using CRL.Model.Memberships;
using CRL.Model.Model.FS;


namespace CRL.Repository.EF.All
{
    public class CBLContext:DbContext
    {
        public CBLContext()
            : base("name=CBLDataContext")
        {
       
    
        }
               

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
       


            #region inheritedTables
            //modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<ActivityDischargeDueToError>().ToTable("ActivityDischargesDueToError");
            modelBuilder.Entity<ActivityDischarge>().ToTable("ActivityDischarges");
            modelBuilder.Entity<ActivityRenewal>().ToTable("ActivityRenewals");  
            modelBuilder.Entity<LegalEntityMembershipRegistrationRequest>().ToTable("LegalEntityMembershipRegistrationRequest");             
             modelBuilder.Entity<WFCaseFS>().ToTable("WFCaseFS");
             modelBuilder.Entity<WFCasePaypointUsersAssignment>().ToTable("WFCasePaypointUsersAssignment");
             modelBuilder.Entity<WFCaseActivity>().ToTable("WFCaseActivity");
             modelBuilder.Entity<WFCasePostpaidSetup>().ToTable("WFCasePostpaidSetup");
             modelBuilder.Entity<WFCaseMembershipRegistration>().ToTable("WFCaseMembershipRegistration");
             modelBuilder.Entity<PaymentAccountTransaction>().ToTable("PaymentAccountTransactions");
             modelBuilder.Entity<CreateFSAccountTransaction>().ToTable("CreateFSAccountTransactions");
             modelBuilder.Entity<ActivityAccountTransaction>().ToTable("ActivityAccountTransactions");
             modelBuilder.Entity<SearchAccountTransaction>().ToTable("SearchAccountTransactions");
             modelBuilder.Entity<PeriodicConfigurationFee>().ToTable("PeriodicConfigurationFees");
             modelBuilder.Entity<PerTransactionConfigurationFee>().ToTable("PerTransactionConfigurationFees");
            #endregion

             #region ConfigurationClasses
             modelBuilder.Configurations.Add(new AddressInfoConfiguration());
             modelBuilder.Configurations.Add(new PersonConfiguration());
             modelBuilder.Configurations.Add(new UserConfiguration());
             modelBuilder.Configurations.Add(new InstitutionConfiguration());
             modelBuilder.Configurations.Add(new MembershipConfiguration());

             modelBuilder.Configurations.Add(new FinancialStatementConfiguration());
             modelBuilder.Configurations.Add(new PersonIdentificationInfoConfiguration());
             modelBuilder.Configurations.Add(new OtherPersonIdentificationInfoConfiguration());
             modelBuilder.Configurations.Add(new IndividualParticipantConfiguration());
             modelBuilder.Configurations.Add(new InstitutionParticipantConfiguration());
             modelBuilder.Configurations.Add(new ParticipantConfiguration());
             modelBuilder.Configurations.Add(new CollateralConfiguration());
             modelBuilder.Configurations.Add(new IndividualSubordinatingPartyConfiguration());
             modelBuilder.Configurations.Add(new InstitutionSubordinatingPartyConfiguration());
             modelBuilder.Configurations.Add(new ActivityAssignmentConfiguration());
             modelBuilder.Configurations.Add(new ActivitySubordinationConfiguration());
             modelBuilder.Configurations.Add(new ActivityUpdateConfiguration());
            
             
             
             #endregion

             //Many to many relation between users and roles
         


      //      modelBuilder.Entity<UserAssignmentFromRolesByReferenceUserRule>().HasRequired(p => p.LimitToRole)
      //       .WithMany()
      //       .HasForeignKey(p => p.LimitToRoleId);


      //      modelBuilder.Entity<UserAssignmentFromRolesByInstitutionsRule>().HasRequired(p => p.LimitToRole)
      //      .WithMany()
      //      .HasForeignKey(p => p.LimitToRoleId);

    //        modelBuilder.Entity<WFUserAssignmentConfigurationRule>().HasMany(p => p.LimitRuleToMembership )
    //       .WithMany();
    //        modelBuilder.Entity<WFUserAssignmentConfigurationRule>().HasMany(p => p.LimitToRoles )
    //    .WithMany();
    //        modelBuilder.Entity<WFUserAssignmentConfigurationRule>().HasMany(p => p.LimitToUsers)
    //.WithMany();
    //        modelBuilder.Entity<WFWorkflowNotificationConfigurationRule>().HasMany(p => p.LimitRuleToMembership)
    //  .WithMany();
    //        modelBuilder.Entity<WFWorkflowNotificationConfigurationRule>().HasMany(p => p.LimitToRoles)
    //    .WithMany();
    //        modelBuilder.Entity<WFWorkflowNotificationConfigurationRule>().HasMany(p => p.LimitToUsers)
    //.WithMany();

            modelBuilder.Entity<WFWorkItem >().HasMany(p => p.AssignedUsers )
  .WithMany();

            modelBuilder.Entity<WFToken>().HasMany(p => p.AssignedUsers)
.WithMany();

          
            modelBuilder.Entity<Report>().HasMany(p => p.Roles )
.WithMany();

            modelBuilder.Entity<Message>().HasMany(p => p.Roles)
.WithMany();
            modelBuilder.Entity<WFCasePaypointUsersAssignment >().HasMany(p => p.Users )
.WithMany();
            modelBuilder.Entity<Message >().HasMany(p => p.ReadBy )
.WithMany();

         
            //modelBuilder.Entity<ServiceRequest>().Property(d => d.RequestNo).HasMaxLength(36).IsFixedLength();

      //      modelBuilder.Entity<UserAssignmentFromUserListRule>().HasMany(p => p.IncludeUsers)
      //  .WithMany();

      //      modelBuilder.Entity<WFTransitionOwnership>().HasMany(p => p.UserAssignmentRules)
      //.WithMany();

//            modelBuilder.Entity<WFTransitionAssignment>().HasMany(p => p.UserAssignmentRules)
//      .WithMany();
//            modelBuilder.Entity<WFTransitionAssignment>().HasMany(p => p.UserNotificationRules )
//.WithMany();
            modelBuilder.Entity<ConfigurationFee >().HasMany(p => p.ServiceFeeType )
.WithMany();
           

          


            modelBuilder.Entity<PasswordResetRequest >().HasOptional(p => p.ResetUser  )
            .WithMany()
            .HasForeignKey(p => p.ResetUserId );
          
            //Collateral Lookups

            modelBuilder.Entity<Collateral>().HasRequired(p => p.AssetType)
             .WithMany()
             .HasForeignKey(p => p.AssetTypeId);

            modelBuilder.Entity<Collateral>().HasRequired (p => p.CollateralSubTypeType )
             .WithMany(c => c.Collaterals).HasForeignKey (p=>p.CollateralSubTypeId);
   


            //modelBuilder.Entity<FileUpload>().HasOptional (p => p.FinancialStatement)
            //.WithMany(c => c.FinancialStatementAttachments)
            //.HasForeignKey(p => p.FinancialStatementId);


            //Financial Statement Activity Lookups
            modelBuilder.Entity<FinancialStatementActivity>().HasRequired(p => p.FinancialStatementActivityType)
           .WithMany()
           .HasForeignKey(p => p.FinancialStatementActivityTypeId);

            modelBuilder.Entity<FinancialStatementActivity>().HasRequired(p => p.FinancialStatement)
         .WithMany(s => s.FinancialStatementActivities)
         .HasForeignKey(p => p.FinancialStatementId);

            modelBuilder.Entity<FinancialStatementActivity>().HasOptional (p => p.PreviousFinancialStatement)
      .WithMany()
      .HasForeignKey(p => p.PreviousFinancialStatementId).WillCascadeOnDelete(false);

        

       

     

       
            //Membership Lookups
      


            ////RepresentativeID for 
    
            //modelBuilder.Entity<RegularMembershipSetting>().HasRequired(p => p.Membership )
            //   .WithMany(b=>b.RegularMembershipSettings).HasForeignKey(p=>p.MembershipId);

            //modelBuilder.Entity<RegularMembershipSetting>().HasOptional(p => p.Represenative)
            //.WithMany().HasForeignKey(p => p.RepresentativeId);


            //Financial Statmement Loopkups
            modelBuilder.Entity<WFCase>().HasRequired(p => p.Workflow )
              .WithMany(b => b.Cases )
              .HasForeignKey(p => p.WorkflowId );

            //Financial Statmement Loopkups
            modelBuilder.Entity<WFTransition >().HasRequired(p => p.Workflow)
              .WithMany(b=>b.Transitions )
              .HasForeignKey(p => p.WorkflowId).WillCascadeOnDelete (false);

            //Financial Statmement Loopkups
            //modelBuilder.Entity<WFWorkItem>().HasRequired(p => p.Workflow)
            //  .WithMany()
            //  .HasForeignKey(p => p.WorkflowId );

            //Financial Statmement Loopkups
            modelBuilder.Entity<WFWorkItem>().HasRequired(p => p.Case  )
              .WithMany( b=>b.WorkItems )
              .HasForeignKey(p => p.CaseId);

            //Financial Statmement Loopkups
            modelBuilder.Entity<WFWorkItem>().HasRequired(p => p.Transition )
              .WithMany()
              .HasForeignKey(p => p.TransitionId );

            //Financial Statmement Loopkups
            //modelBuilder.Entity<WFToken >().HasRequired(p => p.Workflow)
            //  .WithMany()
            //  .HasForeignKey(p => p.WorkflowId);

            //Financial Statmement Loopkups
            modelBuilder.Entity<WFToken >().HasRequired(p => p.Case)
              .WithMany(b => b.Tokens )
              .HasForeignKey(p => p.CaseId);

            //Financial Statmement Loopkups
            modelBuilder.Entity<WFToken>().HasRequired(p => p.Place)
              .WithMany()
              .HasForeignKey(p => p.PlaceId).WillCascadeOnDelete (false);



            modelBuilder.Entity<WFPlace >().HasRequired(p => p.Workflow )
            .WithMany(b => b.Places )
            .HasForeignKey(p => p.WorkflowId);       


            modelBuilder.Entity<WFArc>().HasRequired(p => p.Transition )
              .WithMany(b=>b.Arcs )
              .HasForeignKey(p => p.TransitionId );



            modelBuilder.Entity<WFArc>().HasRequired(p => p.Place )
             .WithMany(b => b.Arcs )
             .HasForeignKey(p => p.PlaceId );
            //Non Auto

            modelBuilder.Entity<LKAssetCategory>().Property(p=>p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKNationality >().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ConfigurationFee>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<LKCountry>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKReportModule>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKReportCategory >().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKLGA>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<LKRegistrationPrefix>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<LKCollateralCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKPersonIdentificationCategory >().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKFinancialStatementActivityCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKFinancialStatementLoanCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKFinancialStatementTransactionCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKMembershipCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKParticipantCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKParticipationCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKMembershipAccountCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LKDebtorCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<RoleCategory>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PublicUserSecurityCode >().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Membership>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ServiceRequest>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Role>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SerialTracker>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None );
            modelBuilder.Entity<FeeLoanSetup>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity  );
            modelBuilder.Entity<ActivityUpdate>().Property(t => t.UpdateXMLDescription).HasColumnType("xml");
            modelBuilder.Entity<SearchFinancialStatement>().Property(t => t.SearchParamXML).HasColumnType("xml");
            modelBuilder.Entity<SearchFinancialStatement>().Property(t => t.CACResultsXML).HasColumnType("xml");


        }

        public DbSet<FinancialStatement> FinancialStatements { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Collateral> Collaterals { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<FinancialStatementActivity> FinancialStatementActivities { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<SearchFinancialStatement> SearchFinancialStatements { get; set; }
        public DbSet<InstitutionUnit> InstitutionUnits { get; set; }
        public DbSet<Membership> Memberships { get; set; }        
        public DbSet<Person> People { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleCategory> RoleCategories { get; set; }
        public DbSet<RoleGroup> RoleGroups { get; set; }
        public DbSet<SerialTracker> SerialTrackers { get; set; }
        public DbSet<WFWorkflow> WFWorkflows { get; set; }
        public DbSet<WFArc> WFArsc { get; set; }
        public DbSet<WFPlace > WFPlaces { get; set; }
        public DbSet<WFTransition> WFTransitions { get; set; }
        public DbSet<FinancialStatementSnapshot> FinancialStatementSnapShots { get; set; }
        public DbSet<WFCase> WFCases { get; set; }        
        public DbSet<WFToken> WFTokens { get; set; }
        public DbSet<WFWorkItem> WFWorkItems { get; set; }
        public DbSet<MembershipRegistrationRequest> MembershipRegistrationRequests { get; set; }
        //public DbSet<WFUserAssignmentConfigurationRule> WFUserAssignmentConfigurationRules { get; set; }      
        //public DbSet<WFWorkflowNotificationConfigurationRule> WFWorkflowNotificationConfigurationRules { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PublicUserSecurityCode> PublicUserSecurityCodes { get; set; }     
        public DbSet<ConfigurationWorkflow> ConfigurationWorkflows { get; set; }
        public DbSet<ConfigurationFee> ConfigurationFees { get; set; }
        public DbSet<WFTask> WFTasks { get; set; }
        public DbSet<WorkflowPlaceAssignmentConfiguration> WorkflowPlaceAssignmentConfigurations { get; set; }
        public DbSet<SubordinatingParty> SubordinatingParty { get; set; }
        public DbSet<FeeLoanSetup> FeeLoanSetups { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<AccountBatch> AccountBatches { get; set; }
        public DbSet<AccountReconcilation> AccountReconcilations { get; set; }
        public DbSet<LKAuditAction> LKAuditAcions { get; set; }
        public DbSet<FSBatch> FSBatches { get; set; }
        public DbSet<FSBatchDetail> FSBatchDetails { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<TempAttachment> TempAttachment { get; set; }
        public DbSet<EmailUserAssignment> EmailUserAssignment { get; set; }
        public DbSet<BankVerificationCode> BankVerificationCodes { get; set; }
        public DbSet<InterSwitchWebPayTransaction> InterSwitchWebPayTransaction { get; set; }
        public DbSet<InterSwitchWebPayTransactionQueryResponse> InterSwitchWebPayTransactionQueryResponse { get; set; }
        public DbSet<InterSwitchDirectPayTransaction> InterSwitchDirectPayTransaction { get; set; }
        public DbSet<InterSwitchDirectPayTransactionQueryResponse> InterSwitchDirectPayTransactionQueryResponse { get; set; }
        public DbSet<TransactionPaymentSetup> TransactionPaymentSetup { get; set; }
        public DbSet<SearchResultTracker> SearchResultTracker { get; set; }
    }
    
}