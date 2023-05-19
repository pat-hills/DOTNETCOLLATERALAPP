using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.FS.IRepository;
using CRL.Model.FS;

namespace CRL.Service.QueryGenerator
{
    public static class FSBatchQueryGenerator
    {
        public static IQueryable<FSBatchView> ViewFSBatchesQuery(ViewBatchRequest request,
       IFSBatchRepository _fsBatchRepository, bool DoCount = false)
        {
            CBLContext ctx = ((FSBatchRepository)_fsBatchRepository).ctx;
            IQueryable<FSBatch> query = ctx.FSBatches.Where (s=>s.IsDeleted ==false );

            if (request.BatchDate !=null)
            {
                query = query.Where(s => s.CreatedOn >= request.BatchDate.StartDate && s.CreatedOn < request.BatchDate.EndDate);
            }

            if (request.ShowType !=null)
                if (request .ShowType ==1)
                    query = query.Where(s => s.FSBatchDetail.Where(e => e.Uploaded == false).Count()>0);
                else if (request.ShowType ==2)
                    query = query.Where(s => s.FSBatchDetail.Where(e => e.Uploaded == false).Count() == 0);
  query = query.Where (s=>s.CreatedByUser .MembershipId == request .SecurityUser .MembershipId );

            var query2 = query.Select(s => new FSBatchView
        {
              Id = s.Id , IsSettled = s.IsSettled , Name = s.Name , NumberOfFS =s.FSBatchDetail .Count (), NumberOfUploadedFS = s.FSBatchDetail .Where (e=>e.Uploaded ==true).Count (),
              RemainingFSToBeUploaded = s.FSBatchDetail .Where (d=>d.Uploaded ==false).Count (), CreatedOn = s.CreatedOn 
        }

      
        );
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.CreatedOn);
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

        public static IQueryable<FSBatchDetail> CreateQueryForFindFSBatchDetail(GetFSFromBatchRequest request,
     IFSBatchRepository _fsBatchRepository, bool DoCount = false)
        {
            CBLContext ctx = ((FSBatchRepository)_fsBatchRepository).ctx;
            IQueryable<FSBatchDetail> query = ctx.FSBatchDetails ;

            if (request.FSBatchId != null)
            {
                query = query.Where(s => s.FSBatchId == request.FSBatchId && s.FSBatch.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);
            }

            
            //if (!DoCount)
            //{
            //    if ((String.IsNullOrWhiteSpace(request.SortColumn)))
            //    {
            //        request.SortColumn = "CreatedOn";
            //        request.SortOrder = "desc";
            //    }

            //    if (request.SortColumn == "CreatedOn")
            //    {
            //        if (request.SortOrder == "desc")
            //        {
            //            query = query.OrderByDescending(s => s.CreatedOn);
            //        }
            //        else
            //        {
            //            query = query.OrderBy(s => s.CreatedOn);
            //        }
            //    }



            //}
            query = query.OrderBy(s => s.Id);
            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering      
      
                query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            
            return query;

        }
    }

}
