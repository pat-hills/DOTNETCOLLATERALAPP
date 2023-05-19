using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;

namespace CRL.WebService.InterswitchPayment.Extensions
{
    public class MyCustomMessageFormatter : IDispatchMessageFormatter
    {
        private readonly IDispatchMessageFormatter formatter;

        public MyCustomMessageFormatter(IDispatchMessageFormatter formatter)
        {
            this.formatter = formatter;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            this.formatter.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var message = this.formatter.SerializeReply(messageVersion, parameters, result);
            return new MyCustomMessage(message);
        }
    }

    public class MyCustomMessage : Message
    {
        private readonly Message message;

        public MyCustomMessage(Message message)
        {
            this.message = message;
        }
        public override MessageHeaders Headers
        {
            get { return this.message.Headers; }
        }
        public override MessageProperties Properties
        {
            get { return this.message.Properties; }
        }
        public override MessageVersion Version
        {
            get { return this.message.Version; }
        }
        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
        }
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            this.message.WriteBodyContents(writer);
        }
        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("SOAP-ENV", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            writer.WriteAttributeString("xmlns", "ns2", null, "http://webservices.ems.momas.com/");
        }
    }
}