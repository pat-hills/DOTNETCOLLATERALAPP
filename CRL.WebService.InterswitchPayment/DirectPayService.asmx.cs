using System;
using System.Web.Services;
using CRL.WebServices.DirectPay;
using CRL.WebServices.DirectPay.Helpers;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Interfaces;
using StructureMap;

namespace CRL.WebService.InterswitchPayment
{
    /// <summary>
    /// Summary description for DirectPayService
    /// </summary>
    [WebService(Namespace = "http://crn.com/webservices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DirectPayService : System.Web.Services.WebService
    {
        [WebMethod]
        public VendResponse Pay(string DestAccount ,decimal Amount ,string Msg ,int SequenceNo ,string DealerNo ,string Password ,string ProductCode ,string Pin )
        {
            VendResponse vendResponse = null;
            var vend = new Vend()
            {
                DestAccount = DestAccount,
                Amount = Amount.ToString(),
                Msg = Msg,
                SequenceNo = SequenceNo.ToString(),
                DealerNo = DealerNo,
                Password = Password,
                ProductCode = ProductCode,
                PIN = Pin
            };

            MakeDirectPayPaymentRequest request = new MakeDirectPayPaymentRequest();
            request.DirectPayRequestView = TransactionMapper.MapToServiceRequest(vend);
            try
            {
                IPaymentService _ps = ObjectFactory.GetInstance<IPaymentService>();
                MakeDirectPayPaymentResponse response = _ps.MakePaymentDirectPayPayment(request);
                vendResponse = TransactionMapper.MapToVendResponse(response.DirectPayResponseView);
            }
            catch(Exception e)
            {
                vendResponse = new VendResponse();
                vendResponse.ResponseCode = "06";
                vendResponse.StatusId = "06";
                vendResponse.ResponseMessage = "Error";
                vendResponse.StatusMessage = "Error";
            }

            return vendResponse;
        }
        
    }
}
