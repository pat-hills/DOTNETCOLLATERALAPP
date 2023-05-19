using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model;
using CRL.Model.ModelViews;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.Messaging;
using CRL.Service.Views;
using CRL.Model.FS.Enums;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Administration;

namespace CRL.Service.QueryGenerator
{
    public class AuditQueryGenerator
    {
        public static IQueryable<AuditView> SelectAudit(
           ViewAuditsRequest request, IAuditRepository _auditRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AuditRepository)_auditRepository).ctx;
            bool LimitToDateRange = request.CreatedRange != null;
            bool OwnerUser = request.SecurityUser.IsOwnerUser; //A
            bool AdminUser = request.SecurityUser.IsInAnyRoles ("Administrator (Client)", "Administrator (Owner)");//B          
            bool LimitToMyRec = request.LimitTo ==true; //C            
            bool ShowOnlyUserRecord = (!OwnerUser && AdminUser && LimitToMyRec) || (!OwnerUser && !AdminUser) || (OwnerUser && AdminUser && LimitToMyRec) || (OwnerUser && !AdminUser ); //Can reduce by mathmatical logic
            bool ShowInstitutionRecord = (!OwnerUser && AdminUser && !LimitToMyRec);
            bool ShowAllRecords = (OwnerUser && AdminUser && !LimitToMyRec);
            bool LimitToMessage = !String.IsNullOrWhiteSpace(request.Message);
            bool LimitToNameOfUser = !String.IsNullOrWhiteSpace(request.NameOfUser);
            bool LimitToNameOfUserLogin = !String.IsNullOrWhiteSpace(request.NameOfUserLogin );
            bool LimitToAuditAction = !String.IsNullOrWhiteSpace(request.AuditAction);
              bool LimitToMachinameName = !String.IsNullOrWhiteSpace(request.MachineName  );
            bool LimitToAuditType = request.AuditTypeId !=null;
    

            IQueryable<Audit> query = _auditRepository.GetDbSet();
            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }

            if (ShowOnlyUserRecord)
            {
                
                query = query.Where(s => s.CreatedByUser.Id == request.SecurityUser.Id);
            }
            else if (ShowInstitutionRecord)
                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);

            
            if (!String.IsNullOrWhiteSpace(request.Message))
            {
                query = query.Where (m=>m.Message.ToLower().Contains (request .Message .ToLower ()));
            }
             if (!String.IsNullOrWhiteSpace(request.NameOfUser))
            {
                query = query.Where (m=> ((m.CreatedByUser.FirstName.TrimEnd() + " " + m.CreatedByUser.MiddleName.TrimStart()).Trim() +
                " " + m.CreatedByUser.Surname.TrimStart()).Trim().ToLower().Contains (request.NameOfUser.ToLower ()));
            }
            if (!String.IsNullOrWhiteSpace(request.NameOfUserLogin ))
            {
                query = query.Where (m=>m.CreatedByUser.Username.ToLower().StartsWith(request.NameOfUserLogin.ToLower ()));
            }

            if (!String.IsNullOrWhiteSpace(request.AuditAction))

            //if (request.AuditAction  != null)
            {
                query = query.Where(m => m.AuditAction .Name .ToLower().Contains(request.AuditAction .ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(request.MachineName))
            //if (request.MachineName  != null)
            {
                query = query.Where(m => m.MachineName.ToLower().Contains(request.MachineName.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.RequestUrl))

            //if (request.RequestUrl != null)
            {
                query = query.Where(m => m.RequestUrl.ToLower().Contains(request.RequestUrl.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.AuditCategoryType))
            //if (request.AuditCategoryType != null)
            {
                //query = query.Where(m => m.RequestUrl.ToLower().Contains(request.RequestUrl.ToLower()));

                query = query.Where(m => m.AuditAction.AuditType.Name.ToLower().Contains(request.AuditCategoryType.ToLower()));

            }

            //if (!String.IsNullOrWhiteSpace(request.AuditTypeId))
             /*if (request.AuditTypeId !=null)
            {
                query = query.Where (m=>m.AuditAction .AuditTypeId  == request.AuditTypeId.Value );
            }*/

            var query2 = query.Select(s => new AuditView
            {
                Id = s.Id ,
                Message = s.Message ,
                AuditDate = s.CreatedOn ,
                AuditAction = s.AuditAction.Name ,
                AuditActionId = s.AuditActionId ,
                RequestUrl= s.RequestUrl ,
                 AuditType = s.AuditAction .AuditType .Name ,
                  AuditTypeId = s.AuditAction .AuditTypeId  ,
                  
                   MachineName = s.MachineName ,
                    NameOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser.MembershipId ).Select(r => r.Name).FirstOrDefault(),
                     UserLoginId = s.CreatedByUser.Username ,
                NameOfUser =( s.CreatedByUser.FirstName.Trim() + " " + s.CreatedByUser.MiddleName ?? "").Trim() + " " + s.CreatedByUser.Surname.Trim()
                  
                //NameOfUser= ctx .People .Where (r=> r.Id == s.CreatedBy ).Select (t=>t.FirstName)         
            });
            //if (!(String.IsNullOrEmpty(request.ClientId)))
            //{
            //    query2 = query2.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientId.ToLower()));
            //    //query3 = query3.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientName.ToLower()));
            //    //query4 = query4.Where(s => s.NameOfLegalEntity.ToLower().Contains(request.ClientName.ToLower()));
            //}
            if (request.ClientId != null && request.ClientId != 0)
            {
                var q = ctx.Institutions.Where(s => s.Id == request.ClientId).Select(r => r.Name).FirstOrDefault();
                query2 = query2.Where(s => s.NameOfLegalEntity.ToLower().Contains(q.ToLower()));

            }
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "AuditDate";
                    request.SortOrder = "desc";
                }
            }
            if (String.IsNullOrWhiteSpace(request.SortColumn)==false)
            {              

                if (request.SortColumn == "AuditDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.AuditDate );
                        //query3 = query3.OrderByDescending(s => s.AuditDate);
                        //query4 = query4.OrderByDescending(s => s.AuditDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.AuditDate );
                        //query3 = query3.OrderBy(s => s.AuditDate);
                        //query4 = query4.OrderBy(s => s.AuditDate);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2= query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;


        }
    }
}
