using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CRL.UI.MVC.Areas.Search.Models.ViewPageModels;
using MvcJqGrid;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Areas.Search.Models.ModelHelpers
{
    

    public static class _FSSearchesViewModelHelper
    {
        public static void MapViewModelToViewFSRequest(_FSSearchesViewModel viewModel, ViewSearchesFSRequest request, GridSettings grid)
        {
            request.SearchDate  = viewModel.GenerateDateRange();
            request.PublicUserCode = viewModel.PublicUserSecurityCode;
            //request.inRequestMode = viewModel.InRequestMode;

            if (grid.IsSearch)
            {
                request.SearchCode = grid.Where.rules.Any(r => r.field == "SearchCode") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "SearchCode").data : string.Empty;
              //  request.RegistrationCode = grid.Where.rules.Any(r => r.field == "ActivityNo") ?
              //  grid.Where.rules.FirstOrDefault(r => r.field == "ActivityNo").data : string.Empty;

              //  request.RequestNo = grid.Where.rules.Any(r => r.field == "RequestNo") ?
              //grid.Where.rules.FirstOrDefault(r => r.field == "RequestNo").data : string.Empty;

                //string FinancialStatementActivityType = grid.Where.rules.Any(r => r.field == "FinancialStatementActivityType") ?
                //    grid.Where.rules.FirstOrDefault(r => r.field == "FinancialStatementActivityType").data : string.Empty;
                //if (FinancialStatementActivityType != String.Empty)
                //{
                //    request.FSActivityCategory = Convert.ToInt32(FinancialStatementActivityType);

                //}

            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
        }
    }
}