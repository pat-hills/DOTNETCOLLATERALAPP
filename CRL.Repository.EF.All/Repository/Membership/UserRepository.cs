using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;
using CRL.Respositoy.EF.All;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.ModelViewMappers;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;
using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.Enums;

namespace CRL.Repository.EF.All.Repository.Memberships
{


    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "User";
        }
        public User GetUserById(int id)
        {
            return ctx.Set<User>().Include("Membership").Include("Roles").Where(s => s.Id == id && s.IsDeleted == false).Single();
        }


        public UserView GetUserViewById(int id)
        {
            User user = ctx.Set<User>().Include("Roles").Include("InstitutionUnit").Include("Membership").Where(s => s.Id == id && s.IsDeleted == false).Single();
            UserView UserView = user.ConvertToUserView();
            string RepresentativeMembershipName = null;
            if (user.Membership.RepresentativeId != null)
            {
                RepresentativeMembershipName = ctx.Institutions.Where(s => s.MembershipId == user.Membership.RepresentativeId).Single().Name;
            }

            UserView.MembershipView = user.Membership.ConvertToMembershipView(RepresentativeMembershipName);

            return UserView;
        }

        public ViewUsersResponse GetUserGrid(ViewUsersRequest request)
        {
            ViewUsersResponse response = new ViewUsersResponse();
            var query = ctx.Set<User>().AsNoTracking().Where(s => s.IsDeleted == false & s.InBuiltUser == false);

            bool LimitToDateRange = request.CreatedRange != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }

            if (!(String.IsNullOrEmpty(request.Username)))
            {
                query = query.Where(s => s.Username.ToLower().StartsWith(request.Username.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Phone)))
            {
                query = query.Where(s => s.Address.Phone.ToLower().StartsWith(request.Phone.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Email)))
            {
                query = query.Where(s => s.Address.Email.ToLower().StartsWith(request.Email.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.ClientCode)))
            {
                query = query.Where(s => s.Membership.ClientCode.ToLower().Contains(request.ClientCode.ToLower()));
            }

            if (!(String.IsNullOrEmpty(request.Fullname)))
            {
                query = query.Where(s => (s.FirstName.ToLower() + " " + (s.MiddleName + " " ?? "") + " " + s.Surname.ToLower()).Contains(request.Fullname.ToLower()));
            }

            if (request.Status != null)
            {
                query = query.Where(s => s.IsActive == request.Status.Value);
            }
            if (!(String.IsNullOrEmpty(request.Unit)))
            {
                query = query.Where(s => s.InstitutionUnit.Name.ToLower().Contains(request.Unit.ToLower()));
            }



            if (request.SecurityUser.InstitutionUnitId != null)
            {
                query = query.Where(s => s.InstitutionUnitId == request.SecurityUser.InstitutionUnitId);
            }
            else
            {

                if (request.InstitutionUnitId != null)
                {
                    query = query.Where(s => s.InstitutionUnitId == request.InstitutionUnitId);
                }
            }

            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.InstitutionId == request.SecurityUser.InstitutionId);
            }
            else
            {
                if (request.InstitutionId != null)
                {
                    query = query.Where(s => s.InstitutionId == request.InstitutionId);
                }

                else if (request.UserListOption == 1)
                {
                    query = query.Where(s => s.InstitutionId == 1);
                }
                else if (request.UserListOption == 2)
                {
                    query = query.Where(s => s.InstitutionId != 1 && s.InstitutionId != null);
                }
            }
            //else
            //{
            //    if (request.UserListOption == 1)
            //    {
            //        query = query.Where(s => s.InstitutionId == 1);
            //    }
            //    else if (request.UserListOption == 2)
            //    {
            //        query = query.Where(s => s.InstitutionId != 1 && s.InstitutionId != null);
            //    }


            //    if (request.LoadOnlyIndividualClients)
            //    {
            //        query = query.Where(s => s.InstitutionId == null);

            //    }

            //    if (request.InstitutionId != null)
            //    {
            //        query = query.Where(s => s.InstitutionId == request.InstitutionId);
            //    }
            //    else
            //    {
            //        query = query.Where(s => s.InstitutionId == request.SecurityUser .InstitutionId );
            //    }

            //    if (request.InstitutionUnitId != null)
            //    {
            //        query = query.Where(s => s.InstitutionUnitId == request.InstitutionUnitId);
            //    }
            //    else if (request.SecurityUser.InstitutionUnitId!=null)
            //    {
            //        query = query.Where(s => s.InstitutionUnitId == request.SecurityUser.InstitutionUnitId);
            //    }

            //}




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

            if (request.AccountStatus != null)
            {
                if (request.AccountStatus.Contains(1))
                {
                    query = query.Where(s => s.IsActive == true && s.IsDeleted == false);
                }
                if (request.AccountStatus.Contains(2))
                {
                    query = query.Where(s => s.IsActive == false && s.IsDeleted == false);
                }
                if (request.AccountStatus.Contains(3))
                {
                    query = query.Where(s => s.IsDeleted == true);
                }
                if (request.AccountStatus.Contains(4))
                {
                    query = query.Where(s => s.IsActive == true && s.IsDeleted == false && (s.IsLockedOut && (Constants.EnableUnlockAfterUnlockMinutes == false || (Constants.EnableUnlockAfterUnlockMinutes == true && s.LastLockedOutDate > DateTime.Now))));
                }
            }

            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
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
                Address = s.Address.Address,
                City = s.Address.City,
                Country = s.Country.Name,
                County = s.County.Name,
                Nationality = s.Nationality.Name,
                MajorityRole = s.Membership.MajorRoleIsSecuredPartyOrAgent == 1 ? "Secured Party" : "Agent",
                CreatedOn = s.CreatedOn,
                IsActive = s.IsActive,
                Unit = s.InstitutionUnit.Name ?? "N/A",
                Status = s.Membership.IsActive == false ? "Pending Membership" :
                s.Membership.IsDeleted == true ? "Denied Membership" :
                s.IsActive == false ? "Deactivated" :
                s.IsLockedOut ? "Locked" :
                s.IsActive == true ? "Active" :
                s.IsDeleted == true ? "Deleted" : "N/A",
                CreatedByUser = (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName ?? "").Trim() + " " + s.CreatedByUser.Surname,
                CreatedByUserFinancingInstitution = s.CreatedByUser.Institution.Name,
                IsLocked = s.IsLockedOut,
                Roles = s.Roles.Select(b => b.Name).ToList(),
                FinancialInstitutionType = s.Institution.Membership.MembershipTypeId != null && (int)s.Institution.Membership.MembershipTypeId == 2 ? "Registry" : "Client Institution",



            });

            if (request.IsReportRequest)
            {
                query2 = query2.OrderByDescending(s => s.CreatedOn);

            }
            else
            {
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
                else
                {
                    query2 = query2.OrderByDescending(s => s.CreatedOn);
                }
            }



            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            response.UserGridView = query2.ToList();
            return response;
        }



        public IQueryable<User> GetNonDeletedUserByLoginId(string LoginId)
        {
            return ctx.Set<User>().Where(s => s.Username.ToLower() == LoginId.ToLower() && s.IsDeleted == false
                );

        }

        public IQueryable<User> GetNonDeletedUserByEmail(string Email)
        {
            return ctx.Set<User>().Where(s => s.Address.Email.ToLower() == Email.ToLower() && s.IsDeleted == false);
        }

        public IQueryable<User> GetDbSetComplete()
        {
            throw new NotImplementedException();
        }
    }
}
