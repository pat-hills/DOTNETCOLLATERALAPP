using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Payments;

namespace CRL.WebServices.DirectPay.Helpers
{
    public static class TransactionMapper
    {
        public static DirectPayRequestView MapToServiceRequest(Vend vend)
        {
            return new DirectPayRequestView()
            {
                DestAccount = vend.DestAccount,
                Amount = Convert.ToDecimal(vend.Amount),
                Msg = vend.Msg,
                SequenceNo = vend.SequenceNo,
                DealerNo = vend.DealerNo,
                Passsword = vend.Password,
                ProductCode = vend.ProductCode,
                Pin = vend.PIN,
                RequestTimestamp = vend.RequestTimestamp
            };
        }

        public static VendResponse MapToVendResponse(DirectPayResponseView DirectPayResponseView)
        {
            return new VendResponse()
            {
                StatusId = DirectPayResponseView.StatusId,
                StatusMessage = DirectPayResponseView.StatusMessage,
                TxRefId = DirectPayResponseView.TxRefId,
                ValueToken = DirectPayResponseView.ValueToken,
                DealerNo = DirectPayResponseView.DealerNo,
                DestAccount = DirectPayResponseView.DestAccount,
                ResponseCode = DirectPayResponseView.ResponseCode == 0 ? "00" : DirectPayResponseView.ResponseCode.ToString(),
                ResponseMessage = DirectPayResponseView.ResponseMessage,
                Amount = DirectPayResponseView.Amount.ToString()
            };
        }

        public static DirectPayReversalRequestView MapToReversalServiceRequest(VendReversal vend)
        {
            return new DirectPayReversalRequestView()
            {
                DestAccount = vend.DestAccount,
                Amount = Convert.ToDecimal(vend.Amount),
                Msg = vend.Msg,
                SequenceNo = vend.SequenceNo,
                DealerNo = vend.DealerNo,
                Passsword = vend.Password,
                ProductCode = vend.ProductCode,
                RequestTimestamp = vend.RequestTimestamp,
                ReceiptNo = vend.RecieptNo
            };
        }

        public static VendReversalResponse MapToReversalVendResponse(DirectPayReversalResponseView DirectPayResponseView)
        {
            return new VendReversalResponse()
            {
                StatusId = DirectPayResponseView.StatusId,
                StatusMessage = DirectPayResponseView.StatusMessage,
                TxRefId = DirectPayResponseView.TxRefId,
                DealerNo = DirectPayResponseView.DealerNo,
                DestAccount = DirectPayResponseView.DestAccount,
                ResponseCode = DirectPayResponseView.ResponseCode == 0 ? "00" : DirectPayResponseView.ResponseCode.ToString(),
                ResponseMessage = DirectPayResponseView.ResponseMessage,
                Amount = DirectPayResponseView.Amount.ToString()
            };
        }
    }
}