using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Views;
using CRL.Service.Views.Payments;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;
using MvcJqGrid;
using System.Configuration;

namespace CRL.UI.MVC.Areas.Payment.Models.ModelHelpers
{
    public static class InterSwitchViewModelHelper
    {
        public static void BuildViewModelForView(InterSwitchUserViewModel viewModel, GetInterSwitchDetailsResponse response)
        {            
            viewModel.InterSwitchUserView = response.InterSwitchTransactionView;
        }

        public static void BuildForCreateEdit(InterSwitchUserViewModel viewModel, SubmitInterSwitchDetailsRequest request)
        {
            request.InterSwitchUserView = new InterSwitchUserView();
            request.InterSwitchUserView = viewModel.InterSwitchUserView;

            request.SiteRedirectUrl = viewModel.InterSwitchUserView.site_redirect_url;
            request.TransactionReference = viewModel.InterSwitchUserView.txn_ref;
        }

        public static void BuildVerifyViewModelForView(InterSwitchTransactionDetailsViewModel viewModel, GetInterSwitchDetailsResponse response)
        {
            viewModel.InterSwitchUserView = new InterSwitchUserView();
            viewModel.InterSwitchUserView = response.InterSwitchTransactionView;
            viewModel.cust_id = response.InterSwitchTransactionView.cust_id;
            viewModel.pay_item_id = response.InterSwitchTransactionView.pay_item_id;
            viewModel.pay_item_name = response.InterSwitchTransactionView.pay_item_name;
            viewModel.product_id = response.InterSwitchTransactionView.product_id;
            viewModel.amount = Math.Round(response.InterSwitchTransactionView.Amount * 100,0);
            viewModel.currency = response.InterSwitchTransactionView.Currency;
            viewModel.site_redirect_url = response.InterSwitchTransactionView.site_redirect_url;
            viewModel.hash = response.InterSwitchTransactionView.hash;
            viewModel.txn_ref = response.InterSwitchTransactionView.txn_ref;
            viewModel.cust_name = response.InterSwitchTransactionView.Name;
        }

        public static void LoadWebPayTransactionTypes(ICollection<TransactionLogType> TransactionLogTypes)
        {
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 0,
                Name = "All",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 1,
                Name = "Successful",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 2,
                Name = "Unsuccessful",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 3,
                Name = "Incomplete",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 4,
                Name = "Pending",
            });
        }

        public static void LoadDirectPayTransactionTypes(ICollection<TransactionLogType> TransactionLogTypes)
        {
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 0,
                Name = "Successful",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 1,
                Name = "Pending",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 2,
                Name = "Reversal",
            });
            TransactionLogTypes.Add(
            new TransactionLogType()
            {
                Id = 3,
                Name = "Unsuccessful",
            });
        }

        public static void MapViewModelToViewInterSwitchTransactions(_InterSwitchTransactionsJqGridViewModel viewModel, GetAllInterSwitchTransactionsRequest request, GridSettings grid)
        {
            request.CreatedRange = viewModel.GenerateDateRange();

            if (grid.IsSearch)
            {
                request.Name = grid.Where.rules.Any(r => r.field == "Name") ?
              grid.Where.rules.FirstOrDefault(r => r.field == "Name").data : string.Empty;

                request.Email = grid.Where.rules.Any(r => r.field == "Email") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Email").data : string.Empty;

                request.Phone = grid.Where.rules.Any(r => r.field == "Phone") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Phone").data : string.Empty;

                string txnDate = grid.Where.rules.Any(r => r.field == "TransactionDate") ?
                 grid.Where.rules.FirstOrDefault(r => r.field == "TransactionDate").data : string.Empty;
                if (!String.IsNullOrEmpty(txnDate))
                {

                    request.TransactionDate = Convert.ToDateTime(txnDate);

                    //request.NewCreatedOn = DateTime.ParseExact(draftDate, "M/dd/yy", null);

                }
            }
            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;
            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.TransactionRefNo = viewModel.TransactionRefNumber;
            request.TransactionLogType = viewModel.SelectedTransactionLogTypeId;
        }

        public static void MapViewModelToViewDirectPayTransactions(_DirectPayTransactionsJqGridViewModel viewModel, GetAllDirectPayTransactionsRequest request, GridSettings grid)
        {
            request.CreatedRange = viewModel.GenerateDateRange();

            if (grid.IsSearch)
            {
                request.Name = grid.Where.rules.Any(r => r.field == "Name") ?
              grid.Where.rules.FirstOrDefault(r => r.field == "Name").data : string.Empty;

                request.Email = grid.Where.rules.Any(r => r.field == "Email") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Email").data : string.Empty;

                request.Phone = grid.Where.rules.Any(r => r.field == "Phone") ?
               grid.Where.rules.FirstOrDefault(r => r.field == "Phone").data : string.Empty;

                string txnDate = grid.Where.rules.Any(r => r.field == "TransactionDate") ?
                 grid.Where.rules.FirstOrDefault(r => r.field == "TransactionDate").data : string.Empty;
                if (!String.IsNullOrEmpty(txnDate))
                {

                    request.TransactionDate = Convert.ToDateTime(txnDate);

                    //request.NewCreatedOn = DateTime.ParseExact(draftDate, "M/dd/yy", null);

                }
            }
            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;
            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.PaymentVoucherCode = viewModel.PaymentVoucherCode;
            request.TransactionLogType = viewModel.SelectedTransactionLogTypeId;
        }
    }
}