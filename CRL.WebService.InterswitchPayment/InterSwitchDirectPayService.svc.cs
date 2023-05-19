using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.WebServices.DirectPay;
using CRL.WebServices.DirectPay.Helpers;
using StructureMap;
using System.ServiceModel.Activation;
using CRL.WebService.InterswitchPayment.Common;

namespace CRL.WebService.InterswitchPayment
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InterSwitchDirectPayService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select InterSwitchDirectPayService.svc or InterSwitchDirectPayService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = "http://webservices.ems.momas.com/")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.NotAllowed)]
    public class InterSwitchDirectPayService : ServiceBase, IInterSwitchDirectPayService
    {
        public VendResponse VendNow(Vend vend)
        {         
            VendResponse vendResponse = null;
            MakeDirectPayPaymentRequest request = new MakeDirectPayPaymentRequest();
            HttpRequestToServiceRequest(request);

            request.DirectPayRequestView = TransactionMapper.MapToServiceRequest(vend);
            try
            {
                IPaymentService _ps = ObjectFactory.GetInstance<IPaymentService>();
                MakeDirectPayPaymentResponse response = _ps.MakePaymentDirectPayPayment(request);
                vendResponse = TransactionMapper.MapToVendResponse(response.DirectPayResponseView);
            }
            catch (Exception ex)
            {
                vendResponse = new VendResponse();
                vendResponse.ResponseCode = "06";
                vendResponse.StatusId = "06";
                vendResponse.ResponseMessage = "Error";
                vendResponse.StatusMessage = "Error";
            }
            
            return vendResponse;
        }

        public VendReversalResponse ReverseVendNow(VendReversal vendReversalModel)
        {
            VendReversalResponse vendReversalResponse = null;
            MakeDirectPayReversalRequest request = new MakeDirectPayReversalRequest();
            HttpRequestToServiceRequest(request);

            request.DirectPayReversalRequestView = TransactionMapper.MapToReversalServiceRequest(vendReversalModel);
            try
            {
                IPaymentService _ps = ObjectFactory.GetInstance<IPaymentService>();
                MakeDirectPayReversalResponse response = _ps.MakeDirectPayReversal(request);
                vendReversalResponse = TransactionMapper.MapToReversalVendResponse(response.DirectPayReversalResponseView);
            }
            catch (Exception e)
            {
                vendReversalResponse = new VendReversalResponse();
                vendReversalResponse.ResponseCode = "06";
                vendReversalResponse.StatusId = "06";
                vendReversalResponse.ResponseMessage = "Error";
                vendReversalResponse.StatusMessage = "Error";
            }
            
            return vendReversalResponse;
        }
    }
}
