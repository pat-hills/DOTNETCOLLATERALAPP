using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model;

using CRL.Service.Views;
using CRL.Infrastructure.Messaging;
using System.Net.Http;

namespace CRL.Service.Messaging
{
    public class ViewMessagesResponse : ResponseBase
    {
        public List<MessageView> MessagesView { get; set; }
        public int NumRecords { get; set; }
    }

    public class ViewMessageResponse : ResponseBase
    {
        public MessageView MessageView { get; set; }

    }

    public class ViewMessagesRequest : PaginatedRequest
    {
        public MessageCategory? MessageCategoryType { get; set; }
        public bool OnlyUnRead { get; set; }
    }

    public class CreateMessagesRequest : RequestBase
    {
        public MessageView MessageView { get; set; }
        public bool AlertUsers { get; set; }
    }

    public static class ConnectToAPI
    {
        public static string Connect(string url)
        {
            string response = "";
            var CACresponse = new HttpResponseMessage();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = TimeSpan.FromSeconds(120);
                try
                {
                    CACresponse = client.GetAsync(url).Result;
                }
                catch
                {
                    response = "Error connectiong to CAC";
                }
                if (CACresponse.IsSuccessStatusCode)
                {
                    if (CACresponse.Content != null)
                    {
                        response = CACresponse.Content.ReadAsStringAsync().Result;
                        if (response.Contains("sqlsrv_query()"))
                        {
                            response = "Error connectiong to CAC";
                        }
                    }
                    else
                    {
                        response = "Error connectiong to CAC";
                    }

                }
                else
                {
                    response = "Error connectiong to CAC";
                }
            }

            return response;
        }
    }

}
