using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Configuration;
using CRL.Model.FS.Enums;

using FluentValidation;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{


  

    
    public class FSViewValidator : AbstractValidator<FSView>
    {
        public FSViewValidator()
        {
           
            RuleFor(x => x.ExpiryDate).Must(MustBeLessThanFiveForConsumerCollaterals).WithMessage("Expiry date cannot be a date that will cause the duration of the registration to exceed 5 years for consumer goods collateral");
            RuleFor(x => x.ExpiryDate).GreaterThanOrEqualTo (DateTime.Now ).WithMessage("Expiry date must be greater than today's date");
            RuleFor(x => x.SecurityAgreementDate).GreaterThanOrEqualTo(DateTime.Now).WithMessage("Loan due date must be greater than today's date");
           // RuleFor(x => Convert.ToDecimal (x.MaximumAmountSecured) ).GreaterThanOrEqualTo(0.01m).WithMessage("Maximum Amount must be more than zero");           
            RuleFor(x => x.SecurityAgreementDate).Must(SecurityAgrementMustHaveValueForConsumableCollateral).WithMessage("Security Agreement Date must be specfied for a consumer collateral");
            RuleFor(x => x.CollateralTypeId).Must(ProperCollateralTypeSet).WithMessage("Please specify a collateral type");
            RuleFor(x => x.MaximumAmountSecuredCurrencyId).NotNull();
           // RuleFor(x => Convert.ToDecimal(x.MaximumAmountSecured)).GreaterThan(0);
            RuleFor(x => x.CollateralTypeId).NotNull();
            RuleFor(x => x.FinancialStatementTransactionTypeId ).NotNull();
            RuleFor(x => x.FinancialStatementLoanTypeId ).NotNull();
            RuleFor(x => x.ParticipantsView).NotNull();
            RuleFor(x => x.CollateralsView ).NotNull();
            RuleFor(x => x.ParticipantsView).Must(HaveAtLeastOneSecuredParty).WithMessage("Must have at least one  secured party");
            RuleFor(x => x.ParticipantsView).Must(HaveAtLeastOneDebtor).WithMessage("Must have at least one  debtor");
            RuleFor(x => x.CollateralsView).Must(HaveAtLeastOneCollateral).WithMessage("Must have at least one collateral");
            RuleFor(x => x.FileAttachments ).Must(HaveFileSizeLimit).WithMessage(String.Format("Cannot exceed file limit of {0} MB",Constants.MaxFileSizeForUpload)).When(t=>t.FileAttachments !=null && t.FileAttachments .Count > 0);
            RuleFor(x => x.ParticipantsView).SetCollectionValidator(new ParticipantViewValidator(true)).When(t => t.FinancialStatementLoanTypeId == 3);
            RuleFor(x => x.ParticipantsView).SetCollectionValidator(new ParticipantViewValidator(false)).When(t => t.FinancialStatementLoanTypeId != 3);
            RuleFor(x => x.CollateralsView).SetCollectionValidator(new CollateralViewValidator());
            
        }
        private bool HaveFileSizeLimit(ICollection<FileUploadView> fileUploadView)
        {
            bool result = true;
            foreach (var f in fileUploadView)
            {
                if (f.AttachedFile != null)
                {
                    result = result & f.AttachedFile.Length <= Constants.MaxFileSizeForUpload * 1024 * 1024;
                }
            }
            return result;
        }
        private bool HaveAtLeastOneSecuredParty(ICollection<ParticipantView> securedparty)
        {
            return securedparty.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).Count() >= 1;
        }
        private bool HaveAtLeastOneDebtor(ICollection<ParticipantView> securedparty)
        {
            return securedparty.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower ).Count() >= 1;
        }
        private bool HaveAtLeastOneCollateral(ICollection<CollateralView> collateral)
        {
            return collateral.Count() >= 1;
        }

        private bool ProperCollateralTypeSet(FSView instance, CollateralCategory  value)
        {
            if (value == CollateralCategory.Both || value == CollateralCategory.CommercialCollateral || value == CollateralCategory.ConsumerGoods)
                return true;
            else
                return false;


            
        }

        private bool SecurityAgrementMustHaveValueForConsumableCollateral(FSView instance, DateTime value)
        {
            if (value == null)
                if ((instance.CollateralTypeId == CollateralCategory.ConsumerGoods || instance.CollateralTypeId == CollateralCategory.Both ) && 
                    instance.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien)
                  return false;
                    
           
                return true;
        }

        private bool MustBeLessThanFiveForConsumerCollaterals(FSView instance, DateTime value)
        {
            //Go through all collaterals

            if (instance.CollateralTypeId == CollateralCategory.ConsumerGoods || instance.CollateralTypeId == CollateralCategory.Both)
                    if (value != null)
                     if ((DateTime.Now.Date.Subtract ((DateTime)value)).Days  > 1800)
                         return false;          
            return true;

           
       
        }

     

       



    }
}