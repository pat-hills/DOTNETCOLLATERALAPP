using CRL.Model.Messaging;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViewValidators.FinancingStatement
{
    public class FSActivityRequestValidator : AbstractValidator<FSActivityRequest>
    {
        public FSActivityRequestValidator()
        {
            RuleFor(x => ((UpdateFSRequest)x).FSView).SetValidator(new FSViewValidator()).When(t => t is UpdateFSRequest); 
        }
    }
}
