using CRL.Infrastructure.Configuration;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.FinancingStatement;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{
    public class ParticipantViewValidator : AbstractValidator<ParticipantView>
    {
        public ParticipantViewValidator(bool IsLien)
        {
            //RuleFor(x => x.Email).EmailAddress().Length(0, 255);
            //RuleFor(x => x.Phone).NotNull().NotEmpty();
            RuleFor(x => x.Address).NotNull().NotEmpty().Length(0, 255);
            RuleFor(x => x.City).NotNull().NotEmpty().Length(0, 70);
            RuleFor(x => x.CountryId).NotNull();
            RuleFor(x => x.CountyId).NotNull();
            RuleFor(x => x.LGAId).NotEmpty().When(t => t.CountryId == Constants.DefaultCountry);
            //RuleFor(x => x.NationalityId).NotNull();            
            RuleFor(x => ((IndividualDebtorView)x).Identification).SetValidator(new IdentificationValidator()).When(t => t is IndividualDebtorView);
            RuleFor(x => ((IndividualDebtorView)x).OtherIdentifications).SetCollectionValidator(new IdentificationValidator()).When(t => t is IndividualDebtorView);
            RuleFor(x => ((IndividualSPView)x).Identification).SetValidator(new IdentificationValidator()).When(t => t is IndividualSPView);
            RuleFor(x => ((IndividualDebtorView)x).Gender).NotNull().NotEmpty().When(t => t is IndividualDebtorView);
            RuleFor(x => ((IndividualSPView)x).Gender).NotNull().NotEmpty().When(t => t is IndividualSPView);
            RuleFor(x => ((IndividualSPView)x).DOB).NotNull().LessThan(DateTime.Now).When(t => t is IndividualSPView);
            RuleFor(x => ((IndividualDebtorView)x).DOB).NotNull().LessThan(DateTime.Now).When(t => t is IndividualDebtorView);

            RuleFor(x => ((IndividualDebtorView)x).CardNo).NotEmpty().When(t => t is IndividualDebtorView).WithMessage("Please enter a BVN");
            
            //RuleFor(x => ((IndividualDebtorParticipantView)x).Identification.OtherDocumentDescription).Must(MustProvideOtherDocumentDescriptionForCollateralWithOther).When(s => s is IndividualDebtorParticipantView).WithMessage("Please provide the other official document for a document type of other"); ;
            //RuleFor(x => ((IndividualDebtorParticipantView)x).OtherIdentifications).Must(MustProvideOtherDocumentDescriptionForOtherIdentificationsWithOther).When(s => s is IndividualDebtorParticipantView).WithMessage("You must provide the other official document for a document type of other for one or more of individual debtors' identification information"); ;
            //RuleFor(x => ((IndividualSPParticipantView)x).Identification.OtherDocumentDescription).Must(MustProvideOtherDocumentDescriptionForCollateralWithOther).When(s => s is IndividualSPParticipantView).WithMessage("Please provide the other official document for a document type of other"); ;
            if (!IsLien)
            {
                RuleFor(x => ((InstitutionDebtorView)x).SectorOfOperationTypes).Must(MustHaveSectorOfOperationForSomeSelectedCompanyType).When(s => s is InstitutionDebtorView)
                    .WithMessage("You must select one or more sectors of operation for all debtor types except government");

            }
            //etc
        }

        private bool MustProvideOtherDocumentDescriptionForOtherIdentificationsWithOther(ParticipantView instance, List<IdentificationView> value)
        {
            if (instance is IndividualDebtorView)
            {
                foreach (var item in value)
                {

                    //if (item.PersonIdentificationTypeId == Constants.OtherIdDocumentConstant && String.IsNullOrEmpty(item.OtherDocumentDescription))
                    //{
                    //    return false;
                    //}
                }
            }
            return true;
        }

        private bool MustProvideOtherDocumentDescriptionForCollateralWithOther(ParticipantView instance, string value)
        {
            if (instance is IndividualSPView)
            {
                //if (((IndividualSPView)instance).Identification.PersonIdentificationTypeId == Constants.OtherIdDocumentConstant && String.IsNullOrEmpty(value))
                //    return false;
                //else
                return true;
            }
            else
            {
                //if (((IndividualDebtorView)instance).Identification.PersonIdentificationTypeId == Constants.OtherIdDocumentConstant && String.IsNullOrEmpty(value))
                //    return false;
                //else
                return true;
            }



        }

        private bool MustHaveSectorOfOperationForSomeSelectedCompanyType(ParticipantView instance, int[] value)
        {
            InstitutionDebtorView pview = (InstitutionDebtorView)instance;

            if (((InstitutionDebtorView)instance).DebtorTypeId != null)
            {
                if (pview.DebtorTypeId != Common.Enum.DebtorCategory.Government &&
                       pview.DebtorTypeId != Common.Enum.DebtorCategory.Other)
                {
                    if (value == null || value.Count() < 1)
                    {
                        return false;
                    }
                }

            }

            return true;

        }
    }

}
