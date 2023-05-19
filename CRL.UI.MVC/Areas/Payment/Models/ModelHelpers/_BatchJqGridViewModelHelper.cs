using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Messaging.Payments.Request;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;
using MvcJqGrid;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Areas.Payment.Models.ModelHelpers
{
    public class _BatchJqGridViewModelHelper
    {
        public static void MapViewModelToViewAccountBatchRequest(_ViewAccountBatchesJqGrid viewModel,ViewAccountBatchesRequest request, GridSettings grid)
        {
            request.TransactionDate = viewModel.GenerateDateRange();

            if (grid.IsSearch)
            {

                string batchId = grid.Where.rules.Any(r => r.field == "Id") ?
                    grid.Where.rules.FirstOrDefault(r => r.field == "Id").data : string.Empty;
                if (String.IsNullOrEmpty(batchId))
                {
                    request.Id = null;
                }
                else {
                    request.Id = Convert.ToInt16(batchId);
                }

                request.BatchStatus = grid.Where.rules.Any(r => r.field == "BatchStatus") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "BatchStatus").data : string.Empty;



                string batchDate = grid.Where.rules.Any(r => r.field == "CreatedOn") ?
                    grid.Where.rules.FirstOrDefault(r => r.field == "CreatedOn").data : string.Empty;
                if (batchDate != String.Empty)
                {
                    request.BatchDate = Convert.ToDateTime(batchDate);
                }
            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;
            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
        }


        public static void CreateViewModelForGrid(_ViewAccountBatchesJqGrid viewModel) {
            viewModel.ReconciliationTypes.Add(new ReconciliationType()
            {
                Value = "fully reconciled",
                Text = "fully reconciled"
            }
                );
            viewModel.ReconciliationTypes.Add(new ReconciliationType()
            {
                Value = "semi-reconciled",
                Text = "semi-reconciled"
            }
            );
            viewModel.ReconciliationTypes.Add(new ReconciliationType()
            {
                Value = "Reconciled (Pending postpaid client confirmation)",
                Text = "Reconciled (Pending postpaid client confirmation)"
            }
            );

            Dictionary<string, string> _rsTypes = new Dictionary<string, string>();
            foreach (var item in viewModel.ReconciliationTypes)
            {
                _rsTypes.Add(item.Value, item.Text);
            }

            viewModel.BatchStatusTypes = _rsTypes;
        }
        
    }
}