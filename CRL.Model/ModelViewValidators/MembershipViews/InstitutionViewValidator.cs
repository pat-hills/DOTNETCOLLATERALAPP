using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using FluentValidation;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.ModelViewValidators.MembershipViews
{
    public class InstitutionViewValidator : AbstractValidator<InstitutionView>
    {
        public InstitutionViewValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Financial Institution name cannot be null");
            RuleFor(x => x.CompanyNo ).NotEmpty().NotNull().WithMessage("Financial Institution number cannot be null");
            RuleFor(x => x.Address).NotEmpty().NotNull().WithMessage("Financial Institution number cannot be null");
            RuleFor(x => x.City).NotEmpty().NotNull().WithMessage("Financial Institution number cannot be null");
            RuleFor(x => x.CountryId).NotEmpty().NotNull().WithMessage("Financial Institution number cannot be null");
            RuleFor(x => x.CountyId ).NotEmpty().NotNull().WithMessage("Financial Institution number cannot be null");
            RuleFor(x => x.SecuringPartyTypeId).NotEmpty().NotNull().WithMessage("Financial Institution name cannot be null");
            //RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Financial Institution email cannot be null").EmailAddress ().WithMessage ("Provide a valid email");
            //RuleFor(x => x.MembershipView ).SetValidator(new MembershipViewValidator ());    
          
        }

    }
}
