using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.ModelViews.Administration;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.UI.MVC.Areas.Administration.Models.ViewPageModels;

namespace CRL.UI.MVC.Areas.Administration.Models.ModelHelpers
{
    public static class AdminViewModelHelper
    {
        public static void BuildViewModelForEditView(GlobalMessageDetailsViewModel viewModel, CreateGMResponse response,bool isEditOrView)
        {
            if (isEditOrView)
            {
                if (response.GlobalMessageDetailsView != null)
                {
                    viewModel._GlobalMessageDetailsViewModel.Id = response.GlobalMessageDetailsView.Id;
                    viewModel._GlobalMessageDetailsViewModel.Title = response.GlobalMessageDetailsView.Title;
                    viewModel._GlobalMessageDetailsViewModel.Body = response.GlobalMessageDetailsView.Body;
                    viewModel._GlobalMessageDetailsViewModel.CreatedOn = response.GlobalMessageDetailsView.CreatedOn;
                    viewModel._GlobalMessageDetailsViewModel.IsLimitedToAdmin =
                        response.GlobalMessageDetailsView.IsLimitedToAdmin;
                    if (response.GlobalMessageDetailsView.IsLimitedToClientOrOwners == 1)
                        viewModel._GlobalMessageDetailsViewModel.IsLimitedToClientOrOwners = true;
                    if (response.GlobalMessageDetailsView.IsLimitedToInstitutionOrIndividual == 1)
                        viewModel._GlobalMessageDetailsViewModel.IsLimitedToInstitutionOrIndividual = true;
                    viewModel._GlobalMessageDetailsViewModel.MessageTypeId =
                        (int) response.GlobalMessageDetailsView.MessageTypeId;
                    viewModel._GlobalMessageDetailsViewModel.SelectedMessageRolesList =
                        response.SelectedMessageRolesList;
                }
            }
            viewModel._GlobalMessageDetailsViewModel.MessageRolesList = response.MessageRolesList;
            
        }

        public static void BuildForCreateEdit(GlobalMessageDetailsViewModel model, CreateSubmitGlobalMessageRequest request)
        {
            request.GlobalMessageDetailsView = new GlobalMessageDetailsView();
            request.GlobalMessageDetailsView.Id = model._GlobalMessageDetailsViewModel.Id;
            request.GlobalMessageDetailsView.Title = model._GlobalMessageDetailsViewModel.Title;
            request.GlobalMessageDetailsView.Body = model._GlobalMessageDetailsViewModel.Body;
            request.GlobalMessageDetailsView.IsLimitedToAdmin = model._GlobalMessageDetailsViewModel.IsLimitedToAdmin;
            if (model._GlobalMessageDetailsViewModel.IsLimitedToClientOrOwners)
            {
                request.GlobalMessageDetailsView.IsLimitedToClientOrOwners = 1;
            }
            else
            {
                request.GlobalMessageDetailsView.IsLimitedToClientOrOwners = 0;
            }
            if (model._GlobalMessageDetailsViewModel.IsLimitedToInstitutionOrIndividual)
            {
                request.GlobalMessageDetailsView.IsLimitedToInstitutionOrIndividual = 1;
            }
            else
            {
                request.GlobalMessageDetailsView.IsLimitedToInstitutionOrIndividual = 0;
            }
            request.GlobalMessageDetailsView.Title = model._GlobalMessageDetailsViewModel.Title;
            request.MessageRoles = model._GlobalMessageDetailsViewModel.MessageRoles;
        }
    }
}