using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views.Workflow;

namespace CRL.Service.Messaging.Workflow.Request
{
  

    public class ViewMyTasksResponse 
    {
        public IEnumerable<TaskGridView> TaskGridView { get; set; }
        public int NumRecords { get; set; }
    }
    
    
}
