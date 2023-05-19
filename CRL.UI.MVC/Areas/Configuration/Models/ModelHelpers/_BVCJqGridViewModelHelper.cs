using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Messaging.Configuration.Request;
using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using MvcJqGrid;

namespace CRL.UI.MVC.Areas.Configuration.Models.ModelHelpers
{
    public static class _BVCJqGridViewModelHelper
    {
        public static void MapViewModelToViewFSRequest(_BvcJqgridViewModel viewModel, GetBvcDataRequest request, GridSettings grid)
        {
            if (grid.IsSearch)
            {
                request.Name = grid.Where.rules.Any(r => r.field == "Name") ?
            grid.Where.rules.FirstOrDefault(r => r.field == "Name").data : string.Empty;

                request.Level = grid.Where.rules.Any(r => r.field == "Level") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "Level").data : string.Empty;

                request.Code = grid.Where.rules.Any(r => r.field == "Code") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "Code").data : string.Empty;

                request.Type = grid.Where.rules.Any(r => r.field == "Type") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "Type").data : string.Empty;
            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
        }
    }
}