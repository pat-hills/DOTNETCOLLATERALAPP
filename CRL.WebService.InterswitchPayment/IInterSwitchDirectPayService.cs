using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CRL.WebServices.DirectPay;
using CRL.WebService.InterswitchPayment.Extensions;

namespace CRL.WebService.InterswitchPayment
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IInterSwitchDirectPayService" in both code and config file together.
    [ServiceContract(Namespace = "http://webservices.ems.momas.com/")]
    public interface IInterSwitchDirectPayService
    {
        [OperationContract(Action = "urn:anonOutInOp")]
        [MobilityProviderFormatMessage]
        VendResponse VendNow(Vend vendModel);

        [OperationContract(Action = "urn:anonOutInReversal")]
        [MobilityProviderFormatMessage]
        VendReversalResponse ReverseVendNow(VendReversal vendRevesalModel);
    }
}
