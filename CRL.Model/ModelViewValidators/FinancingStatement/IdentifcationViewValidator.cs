using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.FinancingStatement;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{
    public class IdentificationValidator : AbstractValidator<IdentificationView>
    {
        public IdentificationValidator()
        {

            RuleFor(x => x.FirstName).NotNull().Length(0, 50).NotEmpty();
            RuleFor(x => x.MiddleName).Length(0, 50);
            RuleFor(x => x.Surname).NotNull().Length(0, 50).NotEmpty();

            
            //RuleFor(x => x.CardNo).NotNull().NotEmpty().Length(0, 25);
            //RuleFor(x => x.PersonIdentificationTypeId).NotNull();
            //RuleFor(x => x.OtherDocumentDescription).NotNull().When(s => s.PersonIdentificationTypeId == Constants.OtherIdDocumentConstant)
            //                                        .NotEmpty().When(s => s.PersonIdentificationTypeId == Constants.OtherIdDocumentConstant);

            // (MustProvideOtherDocumentDescriptionForCollateralWithOther).When(s => s is IndividualDebtorParticipantView).WithMessage("Please provide the other official document for a document type of other"); ;

        }
    }
}
