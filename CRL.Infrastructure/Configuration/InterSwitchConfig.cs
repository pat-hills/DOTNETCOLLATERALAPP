using System;
using System.Collections.Generic;
using System.Linq;
using CRL.Infrastructure.Helpers;
using System.Configuration;
using System.Web;

namespace CRL.Infrastructure.Configuration
{
    public static class InterSwitchConfig
    {
        public static bool IsTempError(string responsCode)
        {            
            return TempErrorList.Contains(responsCode);
        }
        public static readonly List<string> TempErrorList = new List<string>()
        {
            "A0", "Z7", "Z6", "Z4", "Z3", "Z2", "Z1", "Z0", "06"
        };
        private const string InterSwitchXmlApiLive = "https://webpay.interswitchng.com/collections/api/v1/gettransaction.json?";
        //private const string InterSwitchMacKeyLive = "B5EE208CC828167423135AA382B0FDF49FD6C48F298BE67EEE9335765571C549548E7FB10EDDAB28563088A77D3B49F88D46541F3776FAC2EC6ACC1CA196CFDA";
        private const string InterSwitchMacKeyLive = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";
        private const string PurchaseUrlLive = "https://webpay.interswitchng.com/collections/w/pay";
        //private const string Pay_Item_Id_Live = "101";
        //private static int Product_Id_Live = 6228;
        private const string Pay_Item_Id_Live = "101";
        private static int Product_Id_Live = 1076;

        //private const string InterSwitchTestXmlApi = "https://stageserv.interswitchng.com/test_paydirect/api/v1/gettransaction.xml?";
        //private const string InterSwitchTestXmlApi = "https://sandbox.interswitchng.com/collections/api/v1/gettransaction.json";
        private const string InterSwitchTestXmlApi = "https://sandbox.interswitchng.com/collections/api/v1/gettransaction.json?";
        //private const string InterSwitchTestMacKey = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";
        private const string InterSwitchTestMacKey = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";
        //private const string PurchaseUrl_Test = "https://stageserv.interswitchng.com/test_paydirect/pay";
        //private const string PurchaseUrl_Test = "https://sandbox.interswitchng.com/collections/w/pay";
        private const string PurchaseUrl_Test = "https://sandbox.interswitchng.com/collections/w/pay";
        private const string Pay_Item_Id_Test = "101";
        private static int Product_Id_Test = 1076;
        //private const string Pay_Item_Id_Test = "101";
        //private static int Product_Id_Test = 6205;

        public const string CurrencyCode = "566";
        public const string MerchantName = "CBN - National Collateral Registry";
        public const string UnknownTxnRefNo = "Z25";
        public const string SuccessfulTransaction = "00";
        
        public static decimal MinSearchAmt = 100;
        public const string RedirectUrl = "Payment/InterSwitch/Return";

        public const string Password = "Password";
        public const string ProductCode = "6280515091006";

        public static bool OnTestSite
        {
            get
            {
                return ConfigurationManager.AppSettings["onTestSite"] != null ? Boolean.Parse(ConfigurationManager.AppSettings["onTestSite"]) : false;

            }
        }

        public static string InterSwitchXmlApi
        {
            get
            {
                return OnTestSite == true ? InterSwitchTestXmlApi : InterSwitchXmlApiLive;
            }
        }

        public static string InterSwitchMacKey
        {
            get
            {
                return OnTestSite == true ? InterSwitchTestMacKey : InterSwitchMacKeyLive;
            }
        }

        public static string PurchaseUrl
        {
            get
            {
                return OnTestSite == true ? PurchaseUrl_Test : PurchaseUrlLive;
            }
        }

        public static string Pay_Item_Id
        {
            get
            {
                return OnTestSite == true ? Pay_Item_Id_Test : Pay_Item_Id_Live;
            }
        }

        public static int Product_Id
        {
            get
            {
                return OnTestSite == true ? Product_Id_Test : Product_Id_Live;
            }
        }

    }
}
