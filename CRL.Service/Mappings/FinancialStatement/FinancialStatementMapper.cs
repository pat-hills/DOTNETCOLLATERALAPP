using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews;

using CRL.Model.WorkflowEngine;
using CRL.Service.Common;
using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.FinancialStatement
{  

    public static class FinancialStatementMapper
    {








        public static IEnumerable<FSGridView> ConvertToFSGridView(this IEnumerable<Model.FS.FinancialStatement> model, IInstitutionRepository _institutionRepository, IUserRepository _userRepository, int? CurrentUserIdForRequestMode = null)
        {
            ICollection<FSGridView> iview = new List<FSGridView>();
            foreach (var s in model)
            {
                iview.Add(s.ConvertToFSGridView(_institutionRepository, _userRepository, CurrentUserIdForRequestMode));
            }

            return iview;

        }

        public static FSGridView ConvertToFSGridView(this Model.FS .FinancialStatement model, IInstitutionRepository _institutionRepository, IUserRepository _userRepository, int? CurrentUserIdForRequestMode = null)
        {
            FSGridView iview = new FSGridView();

            iview.CollateralTypeId = model.CollateralTypeId;
            iview.RequestNo = model.RequestNo;
            iview.ExpiryDate = model.ExpiryDate;
            iview.FinancialStatementLoanTypeId = model.FinancialStatementLoanTypeId ;
            iview.FinancialStatementTransactionTypeId = model.FinancialStatementTransactionTypeId;
            iview.Id = model.Id;
            iview.MaximumAmountSecured = model.MaximumAmountSecured;
            iview.MaximumAmountSecuredCurrencyId = model.MaximumAmountSecuredCurrencyId;
            iview.RegistrationDate = model.RegistrationDate;
            iview.RegistrationNo = model.RegistrationNo;
            iview.IsDischarged = model.IsDischarged;
            iview.FinancialStatementLastActivity = model.FinancialStatementLastActivity;
            iview.IsPendingAmendment = model.isPendingAmendment;
            iview.IsExpired = model .ExpiryDate <= DateTime .Now ;
           
            if (model.CollateralType != null)
            {
                iview.CollateralTypeName = model.CollateralType.CollateralCategoryName;
            }

            //If we are in request mode and current user is not
            if (CurrentUserIdForRequestMode != null)
            {
                WFCaseFS _case = model.Cases.Where(s => s.IsActive == true && s.IsDeleted == false && s.CaseStatus == "OP").FirstOrDefault();

                if (_case != null)
                {
                    if (_case.WorkItems.Any(w => w.IsActive == true && w.IsDeleted == false && w.WorkitemStatus == "EN" && w.AssignedUsers.Any(u => u.Id == CurrentUserIdForRequestMode)))
                    {
                        iview.CurrentUserIsAssginedToRequest = true;
                    }

                    iview.CaseId =_case.Id;
                    iview.ItemIsLocked = _case.LockItem;
                }
                //if (model.Cases.Any(s => s.IsActive == true && s.IsDeleted == false && s.CaseStatus == "OP" &&
                //    s.WorkItems.Any (w => w.IsActive == true &&  w.IsDeleted ==false && w.WorkitemStatus=="EN" && w.AssignedUsers.Any(u => u.Id == CurrentUserIdForRequestMode))))
                //{
                //    iview.CurrentUserIsAssginedToRequest = true;
                //}

               
            }

            //Lockitems that are in a workflow involving an amendment

            if (iview.FinancialStatementLoanTypeId != null)
            {
                iview.FinancialStatementLoanTypeName = model.FinancialStatementLoanType .FinancialStatementCategoryName;
            }
            iview.FinancialStatementTransactionTypeName = model.FinancialStatementTransactionType .FinancialStatementTransactionCategoryName;
            iview.MembershipId = model.MembershipId;

            //Check if current user can handle

            //**This is bad and should be done in the query rather
            Institution institution = _institutionRepository.GetDbSet ().Where (m=>m.MembershipId == model.MembershipId ).SingleOrDefault ();
            if (institution != null)
                iview.MembershipName = institution.Name ;
            else
            {
                User user = _userRepository.GetDbSet().Where (m=>m.MembershipId == model.MembershipId ).SingleOrDefault ();
                iview.MembershipName = NameHelper.GetFullName (user.FirstName ,user.MiddleName ,user.Surname );
            }

         
            return iview;

        }
       

       
    }
}
