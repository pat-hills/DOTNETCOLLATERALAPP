using System;
using System.Collections.Generic;
using System.Linq;
using CRL.Infrastructure.Messaging;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public static class ReportsViewModelHelper
    {
        public static void MapBaseParametersToRequestBase(ReportBaseViewModel viewModel, ReportRequestBase request)
        {
            request.PageIndex = viewModel.RecordsPage;
            request.NotPaginateRecords = !viewModel.PaginateRecords;
            request.PageSize = viewModel.MaximumRecords == null ? 0 : (int)viewModel.MaximumRecords;
        }

        public static void MapResponseToBaseParameters(ReportBaseViewModel model, ReportResponseBase response)
        {
            if (model.PaginateRecords)
            {
                if (response.NumRecords > model.MaximumRecords)
                {
                    var numOfpages = Math.Ceiling((double)response.NumRecords / model.MaximumRecords);
                    var countPage = 0;
                    while (countPage < numOfpages)
                    {
                        var pageNumber = countPage + 1;
                        model.LimitRecordsOptions.Add(new SelectListItem { Text = pageNumber.ToString(), Value = pageNumber.ToString() });
                        countPage++;
                    }
                }
                else
                {
                    model.LimitRecordsOptions.Add(new SelectListItem { Text = "1", Value = "1" });
                    model.RecordsPage = 1;
                }
            }
        }
    }
}