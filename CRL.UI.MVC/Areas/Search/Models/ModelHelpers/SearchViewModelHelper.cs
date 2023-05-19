using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Model.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Search.Models.ModelHelpers
{
    public class SearchViewModelHelper
    {
        public static void MapToGenerateSearchReport(SearchViewModel viewModel, GenerateSearchReportRequest request)
        {

            request.isCertified = true;
           
            request.publicUsrEmail = viewModel.PublicUserEmail;
            request.Id = viewModel._SearchResultJqGridViewModel.SearchId;
        }
        public static void MapToCreateSubmitRequest(SearchViewModel viewModel, SearchRequest request)
        {
            request.PageIndex = 1;
            request.PageSize = Constants.PageSize;
         
            request.UniqueIdentifier = Util.GetNewValidationCode();
            viewModel._SearchResultJqGridViewModel.UniqueIdentifier = request.UniqueIdentifier;

            string SearchFilter = viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo;

            request.IsNonLegalEffectSearch = viewModel.isNonLegalEffect;            
            request.SearchParam = viewModel._SearchResultJqGridViewModel.SearchParam;


            if (viewModel._SearchResultJqGridViewModel.SearchParam.SearchType == 3)
            {
                request.SearchParam.CollateralSerialNo = SearchFilter;
                request.SearchParam.BorrowerIDNo = null; //Here we are making sure we will not get and and situation if we select a collateral for search
            }

            request.PublicUserEmail = viewModel._SearchResultJqGridViewModel.Email;
            request.PublicUserName = viewModel._SearchResultJqGridViewModel.PublicUserName;
            request.PublicUserBVN = viewModel._SearchResultJqGridViewModel.BVN;
            request.PublicUserPhoneNo = viewModel._SearchResultJqGridViewModel.Phone;
            request.IsPayable = viewModel._SearchResultJqGridViewModel.isPayable;
        }
    }
}