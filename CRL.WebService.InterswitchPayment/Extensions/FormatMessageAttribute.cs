using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace CRL.WebService.InterswitchPayment.Extensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MobilityProviderFormatMessageAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var serializerBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();
            
            if (dispatchOperation.Formatter == null)
            {
                ((IOperationBehavior)serializerBehavior).ApplyDispatchBehavior(operationDescription, dispatchOperation);
            }

            IDispatchMessageFormatter innerDispatchFormatter = dispatchOperation.Formatter;

            dispatchOperation.Formatter = new MyCustomMessageFormatter(innerDispatchFormatter);
        }

        public void Validate(OperationDescription operationDescription) { }
    }
}