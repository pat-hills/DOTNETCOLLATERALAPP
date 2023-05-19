using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Administration;

namespace CRL.Model
{
    public class ViewGlobalMessagesModelRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }
    }

    public class ViewGlobalMessagesModelResponse : ResponseBase
    {
        public ICollection<GlobalMessageView> GlobalMessageView { get; set; }
        public int NumRecords { get; set; }
    }

    public interface IMessageRepository : IWriteRepository<Message, int>
    {
        ViewGlobalMessagesModelResponse GlobalMessageGridView(ViewGlobalMessagesModelRequest request);
        Message GetGlobalMessageDetailsById(int id);
        IEnumerable<Message> GetMessagesByRoleName(string roleName);
    }
   

    public interface ILKMessageCategoryRepository : IWriteRepository<LKMessageCategory , int>
    {
        
    }
}
