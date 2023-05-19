
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Model.WorkflowEngine;
using CRL.Model.Search;
using CRL.Model.Configuration;
using CRL.Model.ModelViews;
using System.ComponentModel.DataAnnotations;
using CRL.Model.FS.Enums;
using CRL.Model.Notification;
using CRL.Infrastructure.Messaging;
using CRL.Model.Memberships;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class FinancialStatement:AuditedEntityBaseModel<int>,IAggregateRoot
    {
        public FinancialStatement()
        {
            this.Participants = new HashSet<Participant>();
            this.Collaterals  = new HashSet<Collateral>();
            this.FinancialStatementActivities = new HashSet<FinancialStatementActivity>();
            this.FileAttachments = new HashSet<FileUpload>();
            this.Cases = new HashSet<WFCaseFS>();
          
        } 
        public string RegistrationNo { get; set; }
        public string RequestNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime SecurityAgreementDate { get; set; }
        public int? MaximumAmountSecuredCurrencyId { get; set; } //*We should have a currency type
        public decimal? MaximumAmountSecured { get; set; }

        public DateTime ExpiryDate { get; set; }
        public string FinancialStatementLastActivity { get; set; }
        public User HandledBy { get; set; }
        public int? HandledById { get; set; }
        public short? isApprovedOrDenied { get; set; }
        public bool isPendingAmendment { get; set; }
        public int? ClonedId { get; set; }

        //Relationship fields
        public System.Nullable <FinancialStatementLoanCategory> FinancialStatementLoanTypeId { get; set; }        
        public virtual LKFinancialStatementLoanCategory FinancialStatementLoanType { get; set; }
        public virtual LKFinancialStatementTransactionCategory FinancialStatementTransactionType { get; set; }
        public FinancialStatementTransactionCategory FinancialStatementTransactionTypeId { get; set; }
        public CollateralCategory CollateralTypeId { get; set; }
        public virtual LKCollateralCategory CollateralType { get; set; }     
        public virtual LKCurrency MaximumAmountSecuredCurrency { get; set; }
        public virtual Membership Membership { get; set; }
        public int MembershipId { get; set; }
        public int? InstitutionUnitId { get; set; }
        public virtual InstitutionUnit InstitutionUnit { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Collateral> Collaterals { get; set; }
        public virtual ICollection<FinancialStatementActivity> FinancialStatementActivities { get; set; }
        //public virtual ICollection<FileUpload> FinancialStatementAttachments { get; set; }
        public virtual ICollection<WFCaseFS> Cases { get; set; }
        public int? VerificationAttachmentId { get; set; }
        public virtual FileUpload VerificationAttachment { get; set; }
        public virtual ICollection<FileUpload> FileAttachments { get; set; }           
        
        public virtual FinancialStatement AfterUpdateFinancialStatement { get; set; }
        public virtual int? AfterUpdateFinancialStatementId { get; set; }
        public int? DischargeActivityId { get; set; }
        public DateTime? SystemExpired { get; set; }

        /// <summary>
        /// Represents the associated discharge object if the collateral is discharged
        /// </summary>
        public virtual ActivityDischarge DischargeActivity { get; set; }
        public bool IsDischarged { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        

        public void Submit(AuditingTracker tracker = null)
        {
            this.IsActive = false;
            FinaliseChildren(RequestMode.Submit, null, tracker);
            tracker.Created.Add(this);
            //this.Audit(tracker);
        }

        public void Update(FinancialStatement previousFS, int AuthorizerId, AuditingTracker tracker = null, IEnumerable<LKSectorOfOperationCategory> _sectorOfOperations = null)
        {

            //Set other data to previous data
            this.State = RecordState.Modified;
            this.MembershipId = previousFS.MembershipId;
            this.InstitutionUnitId = previousFS.InstitutionUnitId;
            this.RegistrationDate = previousFS.RegistrationDate;
            this.EffectiveDate = previousFS.EffectiveDate;
            this.VerificationAttachmentId = previousFS.VerificationAttachmentId;            
            this.CreatedBy = previousFS.CreatedBy;
            this.CreatedOn = previousFS.CreatedOn;
            this.HandledById = previousFS.HandledById;
            this.isApprovedOrDenied = previousFS.isApprovedOrDenied;
            this.IsActive = previousFS.IsActive;
            previousFS.AfterUpdateFinancialStatement = this;
            string ChildrenUniqueCode = Util.GetNewValidationCode() + AuthorizerId.ToString("00000");
            FinaliseChildren(RequestMode.Create, ChildrenUniqueCode, tracker);

            //Add the children that are deleted or not part anymore
            foreach (var c in previousFS.Collaterals)
            {
                var prevc = this.Collaterals.Where(s => s.CollateralNo == c.CollateralNo).SingleOrDefault ();
                
                if (prevc == null)
                {
                    Collateral coll = c.Duplicate();
                    coll.IsDeleted = true;
                    this.Collaterals.Add(coll);                    

                }
                
            }

            foreach (var p in previousFS.Participants)
            {
                var prevc = this.Participants.Where(s => s.ParticipantNo == p.ParticipantNo).SingleOrDefault ();
                if (prevc == null)
                {
                    Participant part = p.Duplicate(null,_sectorOfOperations);
                    part.IsDeleted = true;
                    this.Participants.Add(part);

                }
                //p.CreatedBy = prevc.CreatedBy;
                //p.CreatedOn = prevc.CreatedOn;
                //p.IsActive = prevc.IsActive;
                //p.IsDeleted = prevc.IsDeleted;

                //if (p is IndividualParticipant)
                //{
                //    foreach (var others in ((IndividualParticipant)p).OtherPersonIdentifications)
                //    {

                //        var prevc2 = ((IndividualParticipant)previousFS.Participants).OtherPersonIdentifications.Where(s => s.UniqueCode == others.UniqueCode).Single();
                //        others.CreatedBy = prevc.CreatedBy;
                //        others.CreatedOn = prevc.CreatedOn;
                //        others.IsActive = prevc.IsActive;
                //        others.IsDeleted = prevc.IsDeleted;
                //    }

                //}
            }



        }

        private void MakeActive(int AuthorizerId, AuditingTracker tracker = null)
        {
            this.RegistrationDate = tracker.Date;
            this.EffectiveDate = tracker.Date;
            this.HandledById = AuthorizerId;
            this.isApprovedOrDenied = 1;
            this.IsActive = true;
            
        }

        public void Authorize(int AuthorizerId, AuditingTracker tracker = null)
        {
            this.MakeActive(AuthorizerId,tracker);
            string ChildrenUniqueCode = Util.GetNewValidationCode() + AuthorizerId.ToString("00000");
            FinaliseChildren(RequestMode.Approval , ChildrenUniqueCode, tracker);
            tracker.Updated.Add(this);



        }

        public void Create(int AuthorizerId, AuditingTracker tracker = null)
        {

            this.MakeActive(AuthorizerId, tracker);
            string ChildrenUniqueCode = Util.GetNewValidationCode() + AuthorizerId.ToString("00000");
            FinaliseChildren(RequestMode.Create, ChildrenUniqueCode, tracker);
            tracker.Created.Add(this);



        }

        /// <summary>
        /// Audit children as needed and also assign no
        /// </summary>
        /// <param name="ChildrenUniqueCode"></param>
        /// <param name="tracker"></param>
        public void FinaliseChildren(RequestMode mode, string ChildrenUniqueCode, AuditingTracker tracker = null)
        {
            //Vlidate that we have a unique code when we are in create or authorise mode
            if (mode == RequestMode.Create || mode == RequestMode.Approval)
            {
                if (String.IsNullOrWhiteSpace(ChildrenUniqueCode)) throw new Exception("Was expecting a child unique code when finalising financing statement children");
            }

            bool AuditChild = mode == RequestMode.Submit | mode == RequestMode.Create;
            bool AssignUniqueCode = mode == RequestMode.Create | mode == RequestMode.Approval;

            //Audit new collaterals - audit for submit and create, unique code for create and authorise and authorise date for create and authorise           
            short Indx = 0;
            foreach (var item in Collaterals.Where(s => String.IsNullOrWhiteSpace(s.CollateralNo)))
            {
                if (AuditChild) item.Audit(tracker);
                if (AssignUniqueCode) { Indx++; item.CollateralNo = ChildrenUniqueCode + Indx.ToString("000"); item.AuthorizedDate = tracker.Date; ;}

            }

            //Audit new participants - audit for submit and create, unique code for create and authorise and authorise date for create and authorise           
            Indx = 0;
            foreach (var item in Participants.Where(s => String.IsNullOrWhiteSpace(s.ParticipantNo)))
            {
                if (AuditChild) item.Audit(tracker);
                if (AssignUniqueCode) { Indx++; item.ParticipantNo = ChildrenUniqueCode + Indx.ToString("000"); item.AuthorizedDate = tracker.Date; }
                if (item is IndividualParticipant)
                {
                    short OtherIndex = 0;
                    foreach (var identity in ((IndividualParticipant)item).OtherPersonIdentifications.Where(s => String.IsNullOrWhiteSpace(s.UniqueCode)))
                    {

                        if (AuditChild) identity.Audit(tracker);
                        if (AssignUniqueCode) { OtherIndex++; identity.UniqueCode = ChildrenUniqueCode + OtherIndex.ToString("000"); item.AuthorizedDate = tracker.Date; }
                    }
                }
            }

            if (AuditChild)
            {
                foreach (var file in this.FileAttachments)
                {
                    file.Audit(tracker);
                }
            }

            


        }
        //Methods
        protected override void CheckForBrokenRules()
        {

            throw new NotImplementedException();
        }
        public FinancialStatement Duplicate(IEnumerable<LKSectorOfOperationCategory> _sectorOfOperations = null)
        {

            FinancialStatement fs = new FinancialStatement()
            { Id =this.Id ,
                RegistrationDate = this.RegistrationDate,
                AfterUpdateFinancialStatementId = this.AfterUpdateFinancialStatementId,
                CollateralTypeId = this.CollateralTypeId,
                CreatedBy = this.CreatedBy,
                EffectiveDate = this.EffectiveDate,
                ExpiryDate = this.ExpiryDate,
                UpdatedBy = this.UpdatedBy,
                CreatedOn = this.CreatedOn,
                FinancialStatementLastActivity = this.FinancialStatementLastActivity,
                FinancialStatementLoanTypeId = this.FinancialStatementLoanTypeId,
                FinancialStatementTransactionTypeId = this.FinancialStatementTransactionTypeId,
                isApprovedOrDenied  = this.isApprovedOrDenied ,
                HandledById = this.HandledById ,
                MaximumAmountSecuredCurrencyId = this.MaximumAmountSecuredCurrencyId,
                MembershipId = this.MembershipId,
                RequestNo = this.RequestNo,
                VerificationAttachmentId = this.VerificationAttachmentId,
                MaximumAmountSecured = this.MaximumAmountSecured,
                RegistrationNo = this.RegistrationNo,
                SecurityAgreementDate = this.SecurityAgreementDate,
                
                UpdatedOn = this.UpdatedOn,
                IsActive = this.IsActive,
                IsDeleted = this.IsDeleted,
                 IsDischarged = this.IsDischarged ,
                  isPendingAmendment = this.isPendingAmendment ,
                   ClonedId  =this.Id  ,
                    
            };

            foreach (Participant p in this.Participants )
            {
                fs.Participants.Add(p.Duplicate(fs, _sectorOfOperations));
            }

            foreach (Collateral c in this.Collaterals )
            {
                fs.Collaterals.Add(c.Duplicate(fs));
            }

            return fs;          

        }

        
        public FinancialStatement Duplicate3()
        {
            //Do not do this here better to use the extension in service class perhaps
            return (ObjectCopier.Clone<FinancialStatement>(this));
        }
        public void MarkAsInActive()
        {
            //Do not do this here better to use the extension in service class perhaps
            this.IsActive = false;

            //for all collaterals set to inactive
            this.Collaterals.MakeInActive();
            this.Participants.MakeInActive();
                       
        }

        public void AuditUpdate(int UserId, DateTime Updatedon)
        {
            UpdatedBy =UserId;
            UpdatedOn = Updatedon;
        }       

    }
    [Serializable]
    public partial class LKFinancialStatementLoanCategory : EntityBase<FinancialStatementLoanCategory>, IAggregateRoot
    {
        [MaxLength(70)]
        public string FinancialStatementCategoryName { get; set; }
      


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class LKFinancialStatementTransactionCategory : EntityBase<FinancialStatementTransactionCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string FinancialStatementTransactionCategoryName { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


    [Serializable]
    public partial class FinancialStatementSnapshot : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        //Reference to the finacialstatement before and after amendement

        public byte[] XML { get; set; }
        public short RegistrationOrUpdate { get; set; }
        public short CreateOrEditMode { get; set; }
        public int AssociatedIdForNonNew { get; set; }
        public string ServiceRequest { get; set; }
        
        public string Name { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class TempAttachment : AuditedEntityBaseModel<int>, IAggregateRoot
    {

        public byte[] AttachedFile { get; set; }
        [MaxLength(120)]
        public string AttachedFileName { get; set; }
        [MaxLength(50)]
        public string AttachedFileType { get; set; }
        [MaxLength(50)]
        public string AttachedFileSize { get; set; }
        public string ServiceRequest { get; set; }
        public int? FinancialStatementSnapshotId { get; set; }
        public FinancialStatementSnapshot FinancialStatementSnapshot { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class FileUpload : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public FileUpload()
        {
            FinancialStatements = new HashSet<FinancialStatement>();
            IdentifierId = Guid.NewGuid().ToString("D");
        }        
        //Reference to the finacialstatement before and after amendement
        public byte[] AttachedFile { get; set; }
        [MaxLength(120)]
        public string AttachedFileName { get; set; }
        [MaxLength(150)]
        public string AttachedFileType { get; set; }
        [MaxLength(50)]
        public string AttachedFileSize { get; set; }
        public Email Email { get; set; }
        public int? EmailId { get; set; }
        public virtual ICollection<FinancialStatement> FinancialStatements { get; set; }
        public bool IsTemporaryAttachment { get; set; }
        public string ServiceRequest { get; set; } //Mandatory ofr IsTempAttachment true  
        public string IdentifierId { get; set; }
        public virtual Membership Membership { get; set; }
        public int? MembershipId { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
  
}
