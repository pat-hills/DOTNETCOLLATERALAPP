using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.FS.Enums;
using FluentValidation;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{
    public class CollateralViewValidator : AbstractValidator<CollateralView>
    {
        public CollateralViewValidator()
        {
            RuleFor(x => x.CollateralSubTypeId).NotNull();
            RuleFor(x => x.Description).NotNull().NotEmpty().Length(0, 255).WithMessage("One or more of collaterals requires a serial number ").When(x => x.CollateralSubTypeId == 29 || x.CollateralSubTypeId == 26 || x.CollateralSubTypeId == 43 || x.CollateralSubTypeId == 47 || x.CollateralSubTypeId == 48); //**Change this to the real constant of the motor vehicle
            RuleFor(x => x.SerialNo).NotEmpty().NotNull().Length(0, 100).WithMessage("One or more of collaterals requires a serial number ").When(x => x.CollateralSubTypeId == 29 || x.CollateralSubTypeId == 26 || x.CollateralSubTypeId == 43 || x.CollateralSubTypeId == 47 || x.CollateralSubTypeId == 48); //**Change this to the real constant of the motor vehicle
            
            //etc
        }

      
    }
}