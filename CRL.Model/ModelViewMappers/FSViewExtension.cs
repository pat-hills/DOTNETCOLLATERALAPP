using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;

namespace CRL.Model.ModelViewMappers
{
    public static class FSViewExtension
    {
        public static FS.FinancialStatement ConvertToNewFS(this FSView view, IEnumerable<LKSectorOfOperationCategory> sectors = null, ICollection<FileUpload> ExistingAttachments = null)
        {

            FS.FinancialStatement model = new FS.FinancialStatement();
            model.State = RecordState.New;
            view.ConvertToNewFS(model, sectors,  ExistingAttachments );
            return model;
        }
        public static void ConvertToNewFS(this FSView view, FS.FinancialStatement model, IEnumerable<LKSectorOfOperationCategory> sectors = null, ICollection<FileUpload> ExistingAttachments = null)
        {
            model.State = CRL.Infrastructure.Domain.RecordState.New;
            model.CollateralTypeId = view.CollateralTypeId;
            model.ExpiryDate = view.ExpiryDate;
            if (view.FinancialStatementLoanTypeId != null) { model.FinancialStatementLoanTypeId = (FinancialStatementLoanCategory)view.FinancialStatementLoanTypeId; }
            model.FinancialStatementTransactionTypeId = view.FinancialStatementTransactionTypeId;
            model.MaximumAmountSecured = Convert.ToDecimal(view.MaximumAmountSecured);
            model.MaximumAmountSecuredCurrencyId = view.MaximumAmountSecuredCurrencyId;
            model.RegistrationNo = view.RegistrationNo;
            model.SecurityAgreementDate = view.SecurityAgreementDate;
            model.Participants = view.ParticipantsView.ConvertToNewParticipants(sectors);
            model.Collaterals  = view.CollateralsView .ConvertToNewCollaterals();
            
            //Add attachments from view and those that were temporal
            foreach (var v in view.FileAttachments)
            {
                if (v.Id  == 0)
                {
                    FileUpload fsAttachment = new FileUpload();
                    fsAttachment.AttachedFile = v.AttachedFile;
                    fsAttachment.AttachedFileName = v.AttachedFileName;
                    fsAttachment.AttachedFileSize = v.AttachedFileSize;
                    fsAttachment.AttachedFileType = v.AttachedFileType;                    
                    model.FileAttachments.Add (fsAttachment);

                }
                else 
                {
                    var temp = ExistingAttachments.Where(s => s.Id == v.Id).Single();
                    model.FileAttachments.Add(temp);
                    temp.IsTemporaryAttachment = false;           
                  
                }
            }
            
        }
        public static void ConvertToUpdatedFS(this FSView view, FS.FinancialStatement model, IEnumerable<LKSectorOfOperationCategory> sectors = null, ICollection<FileUpload> ExistingAttachments = null)
        {
            model.State = CRL.Infrastructure.Domain.RecordState.Modified ;
            model.CollateralTypeId = view.CollateralTypeId;
            model.ExpiryDate = view.ExpiryDate;
            if (view.FinancialStatementLoanTypeId != null) { model.FinancialStatementLoanTypeId = (FinancialStatementLoanCategory)view.FinancialStatementLoanTypeId; }
            model.FinancialStatementTransactionTypeId = view.FinancialStatementTransactionTypeId;
            model.MaximumAmountSecured = Convert.ToDecimal(view.MaximumAmountSecured);
            model.MaximumAmountSecuredCurrencyId = view.MaximumAmountSecuredCurrencyId;            
            //model.SecurityAgreementDate = view.SecurityAgreementDate;
            model.Participants = view.ParticipantsView.ConvertToNewParticipants(sectors);


            model.Collaterals = view.CollateralsView.ConvertToUpdatedCollaterals (model.Collaterals );
            

            

            //Load all existing attachments ids so that we do not add them twice and also remove those which are not needed again
            int[] existingAttachmentIds = model.FileAttachments.Select(s => s.Id).ToArray();
             int[] submittedAttachmentIds = view.FileAttachments.Select(s => s.Id).ToArray();

            //Now let's add all new attachments

            //Add attachments from view and those that were temporal
            foreach (var v in view.FileAttachments.Where(s=>s.Id ==0 || s.AttachedFile !=null || !existingAttachmentIds .Contains (s.Id)))
            {

                if (v.AttachedFile != null)
                {
                    FileUpload fsAttachment = new FileUpload();
                    fsAttachment.AttachedFile = v.AttachedFile;
                    fsAttachment.AttachedFileName = v.AttachedFileName;
                    fsAttachment.AttachedFileSize = v.AttachedFileSize;
                    fsAttachment.AttachedFileType = v.AttachedFileType;
                    model.FileAttachments.Add(fsAttachment);

                } 
                //else if (v.TemporalAttachment == true)
                //{
                //    //var temp = tempAttachment.Where(s => s.Id == v.Id).Single(); //**Correct
                //    //model.FileAttachments.Add(temp);
                //   // temp.IsTemporaryAttachment = false;
                //}
            }

            var removedFileAttachments = model.FileAttachments .Where (s=>!submittedAttachmentIds.Contains(s.Id )).ToList ();
            foreach (var removed in removedFileAttachments )
            {
                model.FileAttachments.Remove(removed);
            }

         
         

        }
       
        public static FSView ConvertToFinancialStatementView(this FinancialStatement model)
        {

            FSView iview = new FSView();
            model.ConvertToFinancialStatementView(iview);
            return iview;
        }
        public static void ConvertToFinancialStatementView(this FinancialStatement model, FSView iview)
        {
           iview.CollateralTypeId = model.CollateralTypeId;
            iview.CollateralTypeName = model.CollateralType.CollateralCategoryName;
            iview.ExpiryDate = model.ExpiryDate;
            if (model.FinancialStatementLoanTypeId != null)
                iview.FinancialStatementLoanTypeId = Convert.ToInt32(model.FinancialStatementLoanTypeId);
            iview.FinancialStatementTransactionTypeId = model.FinancialStatementTransactionTypeId;
            iview.Id = model.Id;
            iview.MaximumAmountSecured = string.Format("{0:n}", Convert.ToDecimal(model.MaximumAmountSecured)); 
            
            iview.MaximumAmountSecuredCurrencyId = model.MaximumAmountSecuredCurrencyId;
            iview.RegistrationDate = model.RegistrationDate;
            iview.RegistrationNo = model.RegistrationNo;
            iview.SecurityAgreementDate = model.SecurityAgreementDate;
            
            foreach (var fileuploads in model.FileAttachments)
            {
                FileUploadView fview = new FileUploadView()
                {
                    AttachedFileName = fileuploads.AttachedFileName,
                    AttachedFileSize = fileuploads.AttachedFileSize,
                    AttachedFileType = fileuploads.AttachedFileType,
                    Id = fileuploads.Id
                };

                iview.FileAttachments.Add(fview);
            }
         
            iview.FinancialStatementTransactionTypeName = model.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            if (model.FinancialStatementLoanTypeId != null)
                iview.FinancialStatementLoanTypeName = model.FinancialStatementLoanType.FinancialStatementCategoryName;
            iview.CollateralTypeName = model.CollateralType.CollateralCategoryName;
            iview.CurrencyName = model.MaximumAmountSecuredCurrency.CurrencyName;
            iview.MembershipId = model.MembershipId;
            iview.InstitutionUnitId = model.InstitutionUnitId;
            iview.RowVersion = model.RowVersion;
            iview.IsPendingAmendment = model.isPendingAmendment;
            iview.HasVerificationStatement = model.VerificationAttachmentId != null;
            iview.IsExpired =  DateTime.Now.Date > model.ExpiryDate || model.SystemExpired !=null;
            iview.IsDischarged = model.IsDischarged;
            iview .IsApproved = model.isApprovedOrDenied ==1?true:false;
        }
        
    }
}
