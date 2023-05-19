using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.User.Request;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.QueryGenerator
{
    public static class UserFindQueryGenerator
    {
        public static IQueryable<UserGridView> CreateQueryFor(
           ViewUsersRequest request, IUserRepository _rpIn, bool DoCount)
        {

            CBLContext ctx = ((UserRepository)_rpIn).ctx;

            IQueryable<User> query = _rpIn.GetDbSet();
            query = query.Where(s => s.IsDeleted == false && s.Membership.IsActive == true && s.InBuiltUser == false);
            bool LimitToDateRange = request.CreatedRange != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }

            if (!(String.IsNullOrEmpty(request.Username)))
            {
                query = query.Where(s => s.Username.ToLower().StartsWith (request.Username.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Phone)))
            {
                query = query.Where(s => s.Address.Phone.ToLower().StartsWith(request.Phone.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Email )))
            {
                query = query.Where(s => s.Address .Email .ToLower().StartsWith(request.Email .ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.ClientCode )))
            {
                query = query.Where(s => s.Membership .ClientCode .ToLower().Contains (request.ClientCode .ToLower()));
            }
          
            if (!(String.IsNullOrEmpty(request.Fullname )))
            {
                query = query.Where(s => (s.FirstName.ToLower() +  " " + (s.MiddleName + " " ?? "") + " " + s.Surname.ToLower() ).Contains(request.Fullname .ToLower()));
            }
            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.InstitutionId == request.SecurityUser.InstitutionId);
            }
            else
            {
                if (request.UserListOption == 1 )
                {
                    query = query.Where(s => s.InstitutionId == 1);
                }
                else if (request.UserListOption == 2)
                {
                    query = query.Where(s => s.InstitutionId != 1 && s.InstitutionId != null);
                }


                if (request.LoadOnlyIndividualClients)
                {
                    query = query.Where(s => s.InstitutionId == null);

                }

                if (request.InstitutionId !=null)
                {
                    query = query.Where(s => s.InstitutionId == request.InstitutionId);
                }

            }

            if (request.SecurityUser .InstitutionUnitId  != null)
            {
                query = query.Where(s => s.InstitutionUnitId == request.SecurityUser .InstitutionUnitId );
            }

            if (request.MembershipId > 0)
            {
                query = query.Where(s => s.Membership.Id == request.MembershipId);
            }

            if (request.IsPaypointUser == true)
            {
                query = query.Where(s => s.isPayPointUser == true);
            }

            if (request.IsNonPayPointUser == true)
            {
                query = query.Where(s => s.isPayPointUser == false);
            }
            if (!(String.IsNullOrEmpty(request.ClientName)))
            {
                query = query.Where(s => s.Institution.Name.ToLower().StartsWith(request.ClientName.ToLower()));
               
            }
            var query2 = query.Select(s => new UserGridView
            {
                Id = s.Id,
                ClientCode = s.Membership.ClientCode,
                Email = s.Address.Email,
                FullName = (s.FirstName + " " + s.MiddleName).TrimEnd() + " " + s.Surname,
                Gender = s.Gender,
                InstitutionId = s.InstitutionId,
                Institution = s.Institution != null ? s.Institution.Name : "N/A",
                isPayPoint = s.isPayPointUser,
                Username = s.Username,
                MembershipId = s.MembershipId,
                Phone = s.Address.Phone,
                Address = s.Address .Address ,
                 City = s.Address .City ,
                  Country = s.Country.Name  ,
                   County = s.County.Name ,
                    Nationality = s.Nationality.Name  ,
                       MajorityRole = s.Membership.MajorRoleIsSecuredPartyOrAgent==1 ?"Secured Party": "Agent",
                CreatedOn = s.CreatedOn,
                IsActive = s.IsActive

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
                        query2 = query2.OrderByDescending(s => s.FullName);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.FullName);
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
