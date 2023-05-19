using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Messaging.Payments.Request;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;
using MvcJqGrid;

namespace CRL.UI.MVC.Areas.Payment.Models.ModelHelpers
{
    public class _PaymentJqGridViewModelHelper
    {
        public static void MapViewModelToViewPaymentRequest(_PaymentJqGridViewModel viewModel, ViewPaymentsRequest request, GridSettings grid)
        {
            request.PaymentDate  = viewModel.GenerateDateRange();
            request.ShowType = viewModel.ShowType;
            

            if (grid.IsSearch)
            {
                request.PaymentNo = grid.Where.rules.Any(r => r.field == "PaymentNo") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "PaymentNo").data : string.Empty;

               

                string PaymentType = grid.Where.rules.Any(r => r.field == "PaymentType") ?
                    grid.Where.rules.FirstOrDefault(r => r.field == "PaymentType").data : string.Empty;
                if (PaymentType != String.Empty)
                {
                    request.PaymentTypeId = Convert.ToInt32(PaymentType);

                }

               
            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
        }
    }
}