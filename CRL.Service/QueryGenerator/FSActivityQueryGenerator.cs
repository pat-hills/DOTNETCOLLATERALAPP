using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Model.Messaging;
using CRL.Model.FS.IRepository;

namespace CRL.Service.QueryGenerator
{

    public static class FSActivityQueryGenerator
    {
        public static IQueryable<Model.FS.FinancialStatementActivity> CreateQueryForFindFSActivity(
           ViewFSActivityRequest request, IFinancialStatementActivityRepository _rpFS, bool DoCount)
        {
            IQueryable<Model.FS.FinancialStatementActivity> query = _rpFS.GetDbSet();

            if (!(String.IsNullOrEmpty(request.ActivityCode)))
            {
                query = query.Where(s => s.ActivityCode.ToLower()== request.ActivityCode.ToLower());
            }
            if (!(String.IsNullOrEmpty(request.RegistrationCode)))
            {
                query = query.Where(s => s.FinancialStatement.RegistrationNo .ToLower() == request.RegistrationCode.ToLower());
            }
            if (request.FSActivityCategory > 0)
            {
                query = query.Where(s => s.FinancialStatementActivityTypeId== (FinancialStatementActivityCategory)request.FSActivityCategory);
            }
            if (request.ActivityDate  != null)
            {
                query = query.Where(s => s.CreatedOn >= request.ActivityDate.StartDate && s.CreatedOn < request.ActivityDate.EndDate);
            }
            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            if (request.inRequestMode)
            {
                //We must limit to
                query = query.Where(s => s.isApprovedOrDenied ==null);
                query = query.Where(s => s.Cases.Any(d => d.IsActive == true && d.CaseStatus == "OP"));
            }
            else
            {
                query = query.Where(s => s.isApprovedOrDenied ==1);
            }


            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "UpdatedOn";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "UpdatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.CreatedOn );
                    }
                    else
                    {
                        query = query.OrderBy(s => s.CreatedOn);
                    }
                }

                if (request.SortColumn == "ActivityCode")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.ActivityCode);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.ActivityCode);
                    }
                }

                if (request.SortColumn == "RequestNo")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.RequestNo);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.RequestNo);
                    }
                }
            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            return query;

        }
    }
}
