using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views.Payments;
using CRL.Model.Payments;

namespace CRL.Service.Mappings.Payments
{
    public static class InterSwitchMapper
    {
        public static DirectPayResponseView ConvertToDirectPayResponseView(this InterSwitchDirectPayTransactionQueryResponse InterSwitchDirectPayTransactionQueryResponse)
        {
            return new DirectPayResponseView()
            {
                StatusId = InterSwitchDirectPayTransactionQueryResponse.StatusId,
                StatusMessage = InterSwitchDirectPayTransactionQueryResponse.StatusMessage,
                TxRefId = InterSwitchDirectPayTransactionQueryResponse.TxRefId,
                ValueToken = InterSwitchDirectPayTransactionQueryResponse.ValueToken,
                DealerNo = InterSwitchDirectPayTransactionQueryResponse.DealerNo,
                DestAccount = InterSwitchDirectPayTransactionQueryResponse.DestAccount,
                ResponseCode = InterSwitchDirectPayTransactionQueryResponse.ResponseCode,
                ResponseMessage = InterSwitchDirectPayTransactionQueryResponse.ResponseMessage,      
                Amount = InterSwitchDirectPayTransactionQueryResponse.Amount
            };
        }

        public static InterSwitchDirectPayTransaction ConvertToInterSwitchDirectPayTransaction(this DirectPayRequestView DirectPayRequestView) 
        {
            return new InterSwitchDirectPayTransaction()
            {
                DestAccount = DirectPayRequestView.DestAccount,
                Amount = DirectPayRequestView.Amount,
                Msg = DirectPayRequestView.Msg,
                SequenceNo = DirectPayRequestView.SequenceNo,
                DealerNo = DirectPayRequestView.DealerNo,
                Passsword = DirectPayRequestView.Passsword,
                ProductCode = DirectPayRequestView.ProductCode,
                Pin = DirectPayRequestView.Pin,
                RequestTimeStamp = DirectPayRequestView.RequestTimestamp
            };
        }

        public static void MapToInterSwitchDirectPayTransaction(InterSwitchDirectPayTransaction directPay, DirectPayRequestView DirectPayRequestView) 
        {
            
                directPay.DestAccount = DirectPayRequestView.DestAccount;
                directPay.Amount = DirectPayRequestView.Amount;
                directPay.Msg = DirectPayRequestView.Msg;
                directPay.SequenceNo = DirectPayRequestView.SequenceNo;
                directPay.DealerNo = DirectPayRequestView.DealerNo;
                directPay.Passsword = DirectPayRequestView.Passsword;
                directPay.ProductCode = DirectPayRequestView.ProductCode;
                directPay.Pin = DirectPayRequestView.Pin;
                directPay.RequestTimeStamp = DirectPayRequestView.RequestTimestamp;
            
        }

        public static DirectPayReversalResponseView ConvertToDirectPayReverseResponseView(this InterSwitchDirectPayTransactionQueryResponse InterSwitchDirectPayTransactionQueryResponse)
        {
            return new DirectPayReversalResponseView()
            {
                StatusId = InterSwitchDirectPayTransactionQueryResponse.StatusId,
                StatusMessage = InterSwitchDirectPayTransactionQueryResponse.StatusMessage,
                TxRefId = InterSwitchDirectPayTransactionQueryResponse.TxRefId,
                ValueToken = InterSwitchDirectPayTransactionQueryResponse.ValueToken,
                DealerNo = InterSwitchDirectPayTransactionQueryResponse.DealerNo,
                DestAccount = InterSwitchDirectPayTransactionQueryResponse.DestAccount,
                ResponseCode = InterSwitchDirectPayTransactionQueryResponse.ResponseCode,
                ResponseMessage = InterSwitchDirectPayTransactionQueryResponse.ResponseMessage,
                Amount = InterSwitchDirectPayTransactionQueryResponse.Amount
            };
        }

        public static InterSwitchDirectPayTransaction ConvertToInterSwitchDirectPayTransaction(this DirectPayReversalRequestView DirectPayRequestView)
        {
            return new InterSwitchDirectPayTransaction()
            {
                DestAccount = DirectPayRequestView.DestAccount,
                Amount = DirectPayRequestView.Amount,
                Msg = DirectPayRequestView.Msg,
                SequenceNo = DirectPayRequestView.SequenceNo,
                DealerNo = DirectPayRequestView.DealerNo,
                Passsword = DirectPayRequestView.Passsword,
                ProductCode = DirectPayRequestView.ProductCode,
                Pin = DirectPayRequestView.Pin,
                RequestTimeStamp = DirectPayRequestView.RequestTimestamp,
                ReceiptNo = DirectPayRequestView.ReceiptNo
            };
        }
    }
}
