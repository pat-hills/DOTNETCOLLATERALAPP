using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Messaging.Configuration.Request;
using CRL.UI.MVC.Areas.Administration.Models.ViewPageModels;
using MvcJqGrid;
using CRL.Model;

namespace CRL.UI.MVC.Areas.Administration.Models.ModelHelpers
{
    public static class GlobalMessageViewModelHelper
    {
        public static void MapViewModelToRequest(_GlobalMessageJqGridViewModel viewModel, ViewGlobalMessagesModelRequest request, GridSettings grid)
        {
            request.CreatedRange = viewModel.GenerateDateRange();

            if (grid.IsSearch)
            {
                request.Title = grid.Where.rules.Any(r => r.field == "Title") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Title").data : string.Empty;

                request.Body = grid.Where.rules.Any(r => r.field == "Body") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Body").data : string.Empty;


                string draftDate = grid.Where.rules.Any(r => r.field == "CreatedOn") ?
                 grid.Where.rules.FirstOrDefault(r => r.field == "CreatedOn").data : string.Empty;
                if (!String.IsNullOrEmpty(draftDate))
                {

                    request.NewCreatedOn = Convert.ToDateTime(draftDate);

                    //request.NewCreatedOn = DateTime.ParseExact(draftDate, "M/dd/yy", null);

                }
               

            }
            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            
           
        }
    }
}