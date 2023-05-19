using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Configuration;

namespace CRL.Service.Interfaces
{
    public interface IConfigurationService
    {
        GetWorkflowConfigurationResponse GetDataForConfiguration(GetDataForConfigurationRequest request);
        SaveConfigurationResponse SaveConfiguration(SaveConfigurationRequest request);
        SaveFeeConfigurationResponse SaveFeeConfiguration(SaveFeeConfigurationRequest request);
        GetFeeConfigurationResponse GetFeeConfiguration(RequestBase request);

        ResponseBase ToggleEnableDisableConfiguration(RequestBase request);
        ResponseBase DeleteFeeConfiguration(RequestBase request);
        ResponseBase CreateBVCData(CreateNewBVCDataRequest request);
        GetBvcDataResponse GetBVCData(GetBvcDataRequest request);
        GetFeeResponse GetFeeForPublicSearch(RequestBase request);
        GetDataForConfigurationTransactionFeesResponse GetDataForConfigurationTransactionFees(
            GetDataForConfigurationTransactionFeesRequest request);

        SaveFeesConfigurationResponse SaveFeesConfiguration(SaveFeesConfigurationRequest request);
    }
}
