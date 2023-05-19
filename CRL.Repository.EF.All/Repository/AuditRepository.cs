using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Administration;
using System.Data.Entity;
using CRL.Model.FS.Enums;

namespace CRL.Repository.EF.All.Repository
{


    public class AuditRepository : Repository<Audit, int>, IAuditRepository
    {
        public AuditRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Audit";
        }

        public ViewAuditsResponse CustomViewAudits(ViewAuditsRequest request)
        {
            ViewAuditsResponse response = new ViewAuditsResponse();

            bool LimitToDateRange = request.CreatedRange != null;
            bool OwnerUser = request.SecurityUser.IsOwnerUser; //A
            bool AdminUser = request.SecurityUser.IsInAnyRoles("Administrator (Client)", "Administrator (Owner)"); //B
            bool AdminUnitUser = request.SecurityUser.IsInAnyRoles("Unit Administrator (Client)", "Unit Administrator (Owner)"); //B
            bool LimitToMyRec = request.LimitTo == true; //C

            //bool ShowOnlyUserRecord = (!OwnerUser && AdminUser && LimitToMyRec) || (!OwnerUser && !AdminUser) || (OwnerUser && AdminUser && LimitToMyRec) || (OwnerUser && !AdminUser); //Can reduce by mathmatical logic
            bool ShowOnlyUserRecord = (!AdminUser && !AdminUnitUser) || (LimitToMyRec);
            bool ShowInstitutionRecord = (!OwnerUser && AdminUser && !LimitToMyRec);
            bool ShowUnitRecord = (AdminUnitUser && !LimitToMyRec);
            bool ShowAllRecords = (OwnerUser && AdminUser && !LimitToMyRec);
            bool LimitToMessage = !String.IsNullOrWhiteSpace(request.Message);
            bool LimitToNameOfUser = !String.IsNullOrWhiteSpace(request.NameOfUser);
            bool LimitToNameOfUserLogin = !String.IsNullOrWhiteSpace(request.NameOfUserLogin);
            bool LimitToAuditAction = !String.IsNullOrWhiteSpace(request.AuditAction);
            bool LimitToMachinameName = !String.IsNullOrWhiteSpace(request.MachineName);
            bool LimitToAuditType = request.AuditTypeId != null;


            IQueryable<Audit> query = ctx.Audits.AsNoTracking();
            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }

            if (ShowOnlyUserRecord)
            {

                query = query.Where(s => s.CreatedByUser.Id == request.SecurityUser.Id);
            }
            else if (ShowUnitRecord)
                query = query.Where(s => s.CreatedByUser.InstitutionUnitId == request.SecurityUser.InstitutionUnitId);
            else if (ShowInstitutionRecord)
                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);


            if (!String.IsNullOrWhiteSpace(request.Message))
            {
                query = query.Where(m => m.Message.ToLower().Contains(request.Message.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(request.NameOfUser))
            {
                query = query.Where(m => ((m.CreatedByUser.FirstName.TrimEnd() + " " + m.CreatedByUser.MiddleName.TrimStart()).Trim() +
                " " + m.CreatedByUser.Surname.TrimStart()).Trim().ToLower().Contains(request.NameOfUser.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(request.NameOfUserLogin))
            {
                query = query.Where(m => m.CreatedByUser.Username.ToLower().StartsWith(request.NameOfUserLogin.ToLower()));
            }

            if (request.AuditActionTypes != null && request.AuditActionTypes.Count() > 0)

            //if (request.AuditAction  != null)
            {
                query = query.Where(m => request.AuditActionTypes.Contains((int)m.AuditAction.Id));
            }
            if (!String.IsNullOrWhiteSpace(request.MachineName))
            //if (request.MachineName  != null)
            {
                query = query.Where(m => m.MachineName.ToLower().Contains(request.MachineName.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.GridRequestUrl))
            {
                query = query.Where(m => m.RequestUrl.ToLower().Contains(request.GridRequestUrl.ToLower()));
            }


            if (!String.IsNullOrWhiteSpace(request.NewCreatedOn.ToString()))
            {
                query = query.Where(m => DbFunctions.DiffDays(m.CreatedOn, request.NewCreatedOn) == 0);
            }


            //if (!String.IsNullOrWhiteSpace(request.AuditCategoryType))
            ////if (request.AuditCategoryType != null)
            //{
            //    //query = query.Where(m => m.RequestUrl.ToLower().Contains(request.RequestUrl.ToLower()));

            //    query = query.Where(m => m.AuditAction.AuditType.Name.ToLower().Contains(request.AuditCategoryType.ToLower()));

            //}

            //if (request.AuditTypeId != null && request.AuditTypeId != 0)
            //{
            //    query = query.Where(m => m.AuditAction.AuditType.Id == request.AuditTypeId );
            //}
            if (request.AuditTypeId != null && request.AuditTypeId.Count() > 0)
            {
                query = query.Where(m => request.AuditTypeId.Contains((int)m.AuditAction.AuditType.Id));
            }

            if (request.AuditActionTypes != null && request.AuditActionTypes.Count() > 0)
            {
                query = query.Where(m => request.AuditActionTypes.Contains((int)m.AuditAction.Id));
            }

            if (request.AuditCategory != null && request.AuditCategory != 0)
            {
                query = query.Where(m => m.AuditAction.AuditType.Id == (AuditCategory)request.AuditCategory);
            }


            if (request.AuditActionType != null && request.AuditActionType != 0)
            {
                query = query.Where(m => m.AuditActionId == (AuditAction)request.AuditActionType);
            }

           

            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
            }

            var query2 = query.Select(s => new AuditView
            {
                Id = s.Id,
                Message = s.Message,
                AuditDate = s.CreatedOn,
                AuditAction = s.AuditAction.Name,
                AuditActionId = s.AuditActionId,
                RequestUrl = s.RequestUrl,
                AuditType = s.AuditAction.AuditType.Name,
                AuditTypeId = s.AuditAction.AuditTypeId,

                MachineName = s.MachineName,
                NameOfLegalEntity = s.CreatedBy == 2 ? "Public User" : ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser.MembershipId).Select(r => r.Name).FirstOrDefault(),
                IdOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser.MembershipId).Select(r => r.Id).FirstOrDefault(),
                NameOfUser = (s.CreatedByUser.FirstName.Trim() + " " + s.CreatedByUser.MiddleName ?? "").Trim() + " " + s.CreatedByUser.Surname.Trim(),
                UserLoginId = s.CreatedByUser.Username
                //NameOfUser= ctx .People .Where (r=> r.Id == s.CreatedBy ).Select (t=>t.FirstName)         
            });
            //if (!(String.IsNullOrEmpty(request.ClientId)))
            //{
            //    query2 = query2.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientId));
            //    //query3 = query3.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientName.ToLower()));
            //    //query4 = query4.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientName.ToLower()));
            //}

            if (request.ClientId != null && request.ClientId != 0)
            {
                var q = ctx.Institutions.Where(s => s.Id == request.ClientId).Select(r => r.Name).FirstOrDefault();
                query2 = query2.Where(s => s.NameOfLegalEntity.ToLower().Contains(q.ToLower()));
                
            }

            if ((String.IsNullOrWhiteSpace(request.SortColumn)))
            {
                request.SortColumn = "AuditDate";
                request.SortOrder = "desc";
            }

            if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
            {

                if (request.SortColumn == "AuditDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.AuditDate);
                        //query3 = query3.OrderByDescending(s => s.AuditDate);
                        //query4 = query4.OrderByDescending(s => s.AuditDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.AuditDate);
                        //query3 = query3.OrderBy(s => s.AuditDate);
                        //query4 = query4.OrderBy(s => s.AuditDate);
                    }
                }



            }

            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.AuditViews = query2.ToList();
            return response;
        }

        public ViewAuditDetailsResponse AuditDetails(ViewAuditDetailsRequest request)
        {
            ViewAuditDetailsResponse response = new ViewAuditDetailsResponse();
            Audit audit = ctx.Audits.AsNoTracking().SingleOrDefault(s => s.Id == request.AuditId);
            response.AuditView = new AuditView
            {
                Id = audit.Id,
                Message = audit.Message,
                AuditDate = audit.CreatedOn,
                AuditAction = audit.AuditAction.Name,
                AuditActionId = audit.AuditActionId,
                RequestUrl = audit.RequestUrl,
                AuditType = audit.AuditAction.AuditType.Name,
                AuditTypeId = audit.AuditAction.AuditTypeId,

                MachineName = audit.MachineName,
                NameOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == audit.CreatedByUser.MembershipId).Select(r => r.Name).FirstOrDefault(),
                NameOfUser = (audit.CreatedByUser.FirstName.Trim() + " " + audit.CreatedByUser.MiddleName ?? "").Trim() + " " + audit.CreatedByUser.Surname.Trim()
            };

            return response;
        }
    }
}
