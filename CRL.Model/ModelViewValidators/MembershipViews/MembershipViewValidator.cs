using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using FluentValidation;
using CRL.Model.ModelViews.Enums;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.ModelViewValidators.MembershipViews
{
    public class MembershipViewValidator : AbstractValidator<MembershipView>
    {
        public MembershipViewValidator()
        {
            RuleFor(x => x._MembershipAccountTypeId).NotNull();
            RuleFor(x => x.AccountNumber).NotEmpty().NotNull().When(x => x.MembershipAccountTypeId == MembershipAccountCategory.Regular || x.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative           );
            RuleFor(x => x.RepresentativeMembershipId).NotNull().When(x => x.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative);

          
        }

    }
}
