using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CRL.WebServices.DirectPay
{
    [MessageContract(IsWrapped = true, WrapperName = "Vend", WrapperNamespace = "http://webservices.ems.momas.com/")]
    public class Vend
    {
        [MessageBodyMember(Order = 1, Namespace = "")]
        public string Amount { get; set; }
        [MessageBodyMember(Order = 2, Namespace = "")]
        public string Msg { get; set; }
        [MessageBodyMember(Order = 3, Namespace = "")]
        public string SequenceNo { get; set; }
        [MessageBodyMember(Order = 4, Namespace = "")]
        public string DealerNo { get; set; }
        [MessageBodyMember(Order = 5, Namespace = "")]
        public string Password { get; set; }
        [MessageBodyMember(Order = 6, Namespace = "")]
        public string DestAccount { get; set; }
        [MessageBodyMember(Order = 7, Namespace = "")]
        public string ProductCode { get; set; }
        [MessageBodyMember(Order = 8, Namespace = "")]
        public string PIN { get; set; }
        [MessageBodyMember(Order = 9, Namespace = "")]
        public string RequestTimestamp { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "VendResponse", WrapperNamespace = "http://webservices.ems.momas.com/")]
    public class VendResponse
    {
        [MessageBodyMember(Order = 1)]
        public string StatusId { get; set; }
        [MessageBodyMember(Order = 2)]
        public string StatusMessage { get; set; }
        [MessageBodyMember(Order = 3)]
        public string TxRefId { get; set; }
        [MessageBodyMember(Order = 4)]
        public string DealerNo { get; set; }
        [MessageBodyMember(Order = 5)]
        public string DestAccount { get; set; }
        [MessageBodyMember(Order = 6)]
        public string Amount { get; set; }
        [MessageBodyMember(Order = 7)]
        public string ValueToken { get; set; }
        [MessageBodyMember(Order = 8)]
        public string ResponseCode { get; set; }
        [MessageBodyMember(Order = 9)]
        public string ResponseMessage { get; set; }
    }


    [MessageContract(IsWrapped = true, WrapperName = "VendReversal", WrapperNamespace = "http://webservices.ems.momas.com/")]
    public class VendReversal
    {
        [MessageBodyMember(Order = 1, Namespace = "")]
        public string Amount { get; set; }
        [MessageBodyMember(Order = 2, Namespace = "")]
        public string Msg { get; set; }
        [MessageBodyMember(Order = 3, Namespace = "")]
        public string SequenceNo { get; set; }
        [MessageBodyMember(Order = 4, Namespace = "")]
        public string DealerNo { get; set; }
        [MessageBodyMember(Order = 5, Namespace = "")]
        public string Password { get; set; }
        [MessageBodyMember(Order = 6, Namespace = "")]
        public string DestAccount { get; set; }
        [MessageBodyMember(Order = 7, Namespace = "")]
        public string ProductCode { get; set; }
        [MessageBodyMember(Order = 8, Namespace = "")]
        public string RequestTimestamp { get; set; }
        [MessageBodyMember(Order = 9, Namespace = "")]
        public string RecieptNo { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "VendReversalResponse", WrapperNamespace = "http://webservices.ems.momas.com/")]
    public class VendReversalResponse
    {
        [MessageBodyMember(Order = 1)]
        public string StatusId { get; set; }
        [MessageBodyMember(Order = 2)]
        public string StatusMessage { get; set; }
        [MessageBodyMember(Order = 3)]
        public string TxRefId { get; set; }
        [MessageBodyMember(Order = 4)]
        public string DealerNo { get; set; }
        [MessageBodyMember(Order = 5)]
        public string DestAccount { get; set; }
        [MessageBodyMember(Order = 6)]
        public string Amount { get; set; }
        [MessageBodyMember(Order = 7)]
        public string ResponseCode { get; set; }
        [MessageBodyMember(Order = 8)]
        public string ResponseMessage { get; set; }
    }


}