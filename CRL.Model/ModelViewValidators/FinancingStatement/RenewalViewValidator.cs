using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FluentValidation;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{
    //public class RenewalViewValidator : AbstractValidator<_RenewalViewModel>
    //{
   
    // public RenewalViewValidator()
    //    {
    //        RuleFor(x => x.NewExpiryDate  ).Must(MustBeLessThanFiveForConsumerCollaterals).WithMessage("Duration cannot be more than five years (sixty months) for consumer collateral financing statement");
    //        RuleFor(x => x.NewExpiryDate ).Must(NewAndCurrentAreNotTheSame).WithMessage("The new duration cannot be the same as the old duration");
    // }
    //    private bool MustBeLessThanFiveForConsumerCollaterals(_RenewalViewModel instance,DateTime value)
    //    {
    //        if (instance.IsConsumerCollateral)
    //        {
    //            return ((instance.CurrentExpiryDate.Subtract(value)).Days < 1800);
                      
    //        }
    //        return true;
    //    }

    //    private bool NewAndCurrentAreNotTheSame(_RenewalViewModel instance, DateTime value)
    //    {
    //        if (instance.CurrentExpiryDate .Date  ==value.Date )
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //}
}