using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.ModelViews.Payments;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;

namespace CRL.UI.MVC.Areas.Payment.Models.ModelHelpers
{
    public static class PaymentViewModelViewHelper
    {
        public static void BuildBatchViewModelForView(_ViewBatchDetailsPartailViewModel viewModel, AccountBatchView AccountBatchView)
        {
            viewModel.BatchName = AccountBatchView.Name;
            viewModel.BatchComment = AccountBatchView.Comment;
            viewModel.PeriodStartDate = AccountBatchView.PeriodStartDate;
            viewModel.PeriodEndDate = AccountBatchView.PeriodEndDate;
            viewModel.TotalBatchExpenses = AccountBatchView.TotalExpenses;
            viewModel.TotalSetltement = AccountBatchView.TotalSettlement;
            viewModel.TotalOustandinBatchAmount = AccountBatchView.TotalExpenses - AccountBatchView.TotalExpenses;
        }
    }
}