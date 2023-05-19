
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Views.FinancialStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.QueryGenerator
{
    public class DraftQueryGenerator
    {
        public static IQueryable<DraftView> SelectDraft(
               ViewDraftsRequest request, IFinancialStatementSnapshotRepository _financialStatementSnapshotRepository, bool DoCount = false)
        {
            CBLContext ctx = ((FinancialStatementSnapshotRepository)_financialStatementSnapshotRepository).ctx ;

            var query = ctx.FinancialStatementSnapShots.AsNoTracking().Where(s => s.CreatedBy == request.SecurityUser.Id);

            if (!(String.IsNullOrEmpty(request.DraftName)))
            {
                query = query.Where(s => s.Name.ToLower().Contains(request.DraftName));
            }



            var query2 = query.Select(s => new DraftView
              {
                  Id = s.Id,
                  AssociatedIdForNonNew = s.AssociatedIdForNonNew,
                  CreateOrEditMode = s.CreateOrEditMode,
                  Name = s.Name,
                  ActionWhenDraftWasCreated = s.CreateOrEditMode == 2 && s.RegistrationOrUpdate == 1 ? "Creating new financing statement" :
                   s.CreateOrEditMode == 3 && s.RegistrationOrUpdate == 1 ? "Resubmitting new financing statement" :
                     s.CreateOrEditMode == 2 && s.RegistrationOrUpdate == 2 ? "Updating existing financing statement" :
                     s.CreateOrEditMode == 3 && s.RegistrationOrUpdate == 2 ? "Resubmitting update of existing financing statement" : "N/A",
                  RegistrationOrUpdate = s.RegistrationOrUpdate,
                  ServiceRequest = s.ServiceRequest,
                  CreatedOn = s.CreatedOn
              })
                  ;



            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }
            }
            if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
            {

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.CreatedOn );
                        //query3 = query3.OrderByDescending(s => s.AuditDate);
                        //query4 = query4.OrderByDescending(s => s.AuditDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.CreatedOn);
                        //query3 = query3.OrderBy(s => s.AuditDate);
                        //query4 = query4.OrderBy(s => s.AuditDate);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;
        }
    }
}
