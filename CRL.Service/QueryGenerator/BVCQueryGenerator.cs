using CRL.Infrastructure.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Messaging.Configuration.Request;

namespace CRL.Service.QueryGenerator
{
    public class BVCQueryGenerator
    {
        public static IQueryable<BankVerificationCodeView> CreateQueryFor(
         GetBvcDataRequest request, IBankVerificationCodeRepository _rpIn, bool DoCount = false)
        {

            CBLContext ctx = ((BankVerificationCodeRepository)_rpIn).ctx;

            IQueryable<BankVerificationCode> query = _rpIn.GetDbSet();
            query = query.Where(s => s.IsDeleted == false && s.IsActive == true);

            if (!(String.IsNullOrEmpty(request.Name)))
            {
                query = query.Where(s => s.Name.ToLower().Contains(request.Name.ToLower()));
            }

            if (!(String.IsNullOrEmpty(request.Level)))
            {
                query = query.Where(s => s.level.ToLower().Contains(request.Level.ToLower()));
            }

            if (!(String.IsNullOrEmpty(request.Code)))
            {
                query = query.Where(s => s.Code.ToLower().Contains(request.Code.ToLower()));
            }

            if (!(String.IsNullOrEmpty(request.Type)))
            {
                query = query.Where(s => s.Type.ToLower().Contains(request.Type.ToLower()));
            }

            var query2 = query.Select(s => new BankVerificationCodeView
            {
                Code = s.Code,
                Name = s.Name,
                Level = s.level,
                Type = s.Type
            });
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "Name";
                    request.SortOrder = "asc";
                }
            }

            if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
            {

                if (request.SortColumn == "Name")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Name);
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
