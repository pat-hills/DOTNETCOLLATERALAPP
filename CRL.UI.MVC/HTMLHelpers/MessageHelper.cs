using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Messaging;
using CRL.UI.MVC.Common;
namespace CRL.UI.MVC.HTMLHelpers
{
    public static class MessageHelper
    {
        /// <summary>
        /// Returns an mvc html string from the MessageInfo class
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        private static string GetMvcHtmlStringFromMessage(MessageInfo Message)
        {
            string class_style = "msg info"; //for information message                

            if (Message.MessageType == MessageType.Info)
            {
                class_style = "msg info";

            }
            else if (Message.MessageType == MessageType.Warning)
            {
                class_style = "msg warning";

            }
            else if (Message.MessageType == MessageType.Error)
            {
                class_style = "msg error";

            }
            else if (Message.MessageType == MessageType.Success)
            {
                class_style = "msg done";

            }
            else if (Message.MessageType == MessageType.BusinessValidationError)
            {
                class_style = "msg error";

            }

            TagBuilder divTag = new TagBuilder("div");
            TagBuilder pTag = new TagBuilder("p");
            pTag.AddCssClass(class_style);
            //REPLACE TAGS WITH THE FOLLWOING
            //BR 
            pTag.InnerHtml = Message.Message;
            divTag.InnerHtml = pTag.ToString();

            //**Consider using this again.  We will form tags ourselves
            return divTag.ToString();

        }
        /// <summary>
        /// Extension method to the HtmlHelper which enables us to get the Mvc String from a single MessageInfo class
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static MvcHtmlString Message(this HtmlHelper html, MessageInfo Message)
        {

            return new MvcHtmlString(GetMvcHtmlStringFromMessage(Message));
        }



        /// <summary>
        /// Extension method to the HtmlHelper which enables us to get the Mvc String from a List of MessageInfo class
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static MvcHtmlString MessageList(this HtmlHelper html, List<MessageInfo> Message)
        {

            string MessageString = "";
            foreach (MessageInfo message in Message)
            {
                MessageString += GetMvcHtmlStringFromMessage(message);
            }

            return new MvcHtmlString(MessageString);
        }
    }
}