using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Common;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Respositoy.EF.All;
using CRL.Respositoy.EF.All.Common.Repository;
using System.Data.Entity;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Repository.EF.All.Repository.Memberships
{



    public class InstitutionRepository : Repository<Institution, int>, IInstitutionRepository
    {
        public InstitutionRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Institution";
        }

        public Institution GetInstitutionMembershipById(int id)
        {
            return ctx.Institutions.Include("Membership").
              Where(s => s.Id == id).Single();

        }

        public Institution GetInstitutionDetailById(int id)
        {
            return ctx.Institutions.Include("Country").Include("Nationality").Include("SecuringPartyType").Include("Membership").
             Where(s => s.Id == id).Single();
        }

        public GetCreateEditInstitutionResponse GetCreateEditInstitution()
        {
            GetCreateEditInstitutionResponse response = new GetCreateEditInstitutionResponse();
            var cmd = ctx.Database.Connection.CreateCommand();
            cmd.CommandText = "[dbo].[GetCreateEditInstitution]";

            try
            {
                // Run the sproc
                ctx.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                // Read Blogs from the first result set
                var countries = ((IObjectContextAdapter)ctx)
                    .ObjectContext
                    .Translate<LKCountry>(reader, "LKCountries", MergeOption.AppendOnly);

                foreach (var c in countries)
                {
                    LookUpView lk = new LookUpView();
                    lk.LkValue = (int)c.Id;
                    lk.LkName = c.Name;
                    response.Countries.Add(lk);
                }

                // Move to second result set and read Posts
                reader.NextResult();
                var countys = ((IObjectContextAdapter)ctx)
                    .ObjectContext
                    .Translate<LKCounty>(reader, "LKCounties", MergeOption.AppendOnly);

                foreach (var c in countys)
                {
                    LookUpView lk = new LookUpView();
                    lk.LkValue = (int)c.Id;
                    lk.LkName = c.Name;
                    response.Countys.Add(lk);
                }

                // Move to second result set and read Posts
                reader.NextResult();
                var nationalities = ((IObjectContextAdapter)ctx)
                    .ObjectContext
                    .Translate<LKNationality>(reader, "LKNationalities", MergeOption.AppendOnly);
                foreach (var c in nationalities)
                {
                    LookUpView lk = new LookUpView();
                    lk.LkValue = (int)c.Id;
                    lk.LkName = c.Name;
                    response.Nationalities.Add(lk);
                }

                // Move to second result set and read Posts
                reader.NextResult();
                var securingPartyIndustryCategories = ((IObjectContextAdapter)ctx)
                    .ObjectContext
                    .Translate<LKSecuringPartyIndustryCategory>(reader, "LKSecuringPartyIndustryCategories", MergeOption.AppendOnly);
                foreach (var c in securingPartyIndustryCategories)
                {
                    LookUpView lk = new LookUpView();
                    lk.LkValue = (int)c.Id;
                    lk.LkName = c.SecuringPartyIndustryCategoryName;
                    response.SecuringPartyTypes.Add(lk);
                }

                // Move to second result set and read Posts
                reader.NextResult();
                var institutions = ((IObjectContextAdapter)ctx)
                    .ObjectContext
                    .Translate<Institution>(reader, "Institutions", MergeOption.AppendOnly);
                foreach (var c in institutions)
                {
                    LookUpView lk = new LookUpView();
                    lk.LkValue = (int)c.Id;
                    lk.LkName = c.Name;
                    response.RegularClientInstitutions.Add(lk);
                }

            }
            finally
            {
                ctx.Database.Connection.Close();
            }

            return response;
        }

        /// <summary>
        /// This query is used to get a view for Institution with MembershipView info loaded
        /// In our next update it would be preferable to return flat classes where
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InstitutionView InstitutionMembershipViewById(int id)
        {
            var query = ctx.Institutions.AsNoTracking().Where(m => m.Id == id);
            var select_query = query.Select(s => new InstitutionView
            {
                Name = s.Name,
                Email = s.Address.Email,
                Phone = s.Address.Phone,
                Address = s.Address.Address,
                Address2 = s.Address.Address2,
                City = s.Address.City,
                CountryId = s.CountryId,
                County = s.County.Name,
                CountyId = s.CountyId,
                Country = s.Country.Name,
                LGA = s.LGA.Name,
                LGAId = s.LGAId,
                CompanyNo = s.CompanyNo,
                SecuringPartyTypeId = s.SecuringPartyTypeId,
                SecuringPartyType = s.SecuringPartyType.SecuringPartyIndustryCategoryName,
                NationalityId = s.NationalityId,
                Nationality = s.Nationality.Name,
                Id = s.Id,
                MembershipView = new MembershipView
                {
                    Id = s.Membership.Id,
                    MembershipGroup = s.Membership.MembershipGroup,
                    MembershipType = s.Membership.MembershipType.MembershipCategoryName,
                    MembershipTypeId = s.Membership.MembershipTypeId,
                    MembershipAccountTypeId = s.Membership.MembershipAccountTypeId,
                    _MembershipAccountTypeId = (int)(s.Membership.MembershipAccountTypeId),
                    MembershipAccountTypeName = s.Membership.MembershipAccountType.MembershipAccountCategoryName,
                    CreditCode = s.Membership.ClientCode,
                    PrepaidCreditBalance = s.Membership.PrepaidCreditBalance,
                    PostpaidCreditBalance = s.Membership.PostpaidCreditBalance,
                    RepresentativeMembershipId = s.Membership.RepresentativeId,
                    AccountNumber = s.Membership.AccountNumber,
                    isPayPointClient = s.Membership.isPayPointClient,
                    RepresentativeMembership = s.Membership.RepresentativeId != null,
                    MajorRoleIsSecuredPartyOrAgent = s.Membership.MajorRoleIsSecuredPartyOrAgent,
                    RepresentativeMembershipName = ctx.Institutions.Where(b => b.Id == s.Membership.RepresentativeId).FirstOrDefault().Name

                }
            });

            return select_query.Single();
        }


        public ViewClientInstitutionsResponse InstitutionGridView(ViewClientInstitutionsRequest request)
        {
            ViewClientInstitutionsResponse response = new ViewClientInstitutionsResponse();

            var query = this.InstitutionListHelper(request);
            if (request.IsReportRequest)
            {
                if (request.SecuredPartyType != null)
                {
                    query = query.Where(s => s.SecuringPartyTypeId == request.SecuredPartyType);
                }

                if (request.SecuredPartyTypes != null && request.SecuredPartyTypes.Count() > 0)
                {
                    query = query.Where(s => s.SecuringPartyTypeId != null && request.SecuredPartyTypes.Contains((int)s.SecuringPartyTypeId));
                }
            }
            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
            }
            var query2 = query.Select(s => new InstitutionGridView
            {
                Id = s.Id,
                ClientCode = s.Membership.ClientCode,
                Email = s.Address.Email != null ? s.Address.Email : "N/A",
                Address = s.Address.Address,
                City = s.Address.City,
                CompanyNo = s.CompanyNo,
                Country = s.Country.Name,
                County = s.County.Name,
                Phone = s.Address.Phone,
                Name = s.Name,
                Nationality = s.Nationality.Name,
                SecuredPartyType = s.SecuringPartyType.SecuringPartyIndustryCategoryName,
                MembershipAccountType = s.Membership.MembershipAccountType.MembershipAccountCategoryName,
                CreatedOn = s.CreatedOn,
                IsActive = s.IsActive,
                MajorityRole = s.Membership.MajorRoleIsSecuredPartyOrAgent == 1 ? "Secured Party" : "Agent",
                MembershipId = s.MembershipId,
                MembershipStatus = s.Membership.IsActive ? 0 : s.Membership.IsDeleted ? 2 : 1,
                Address2 = s.Address.Address2 != null ? s.Address.Address2 : "N/A",
                LGA = s.LGA.Name,
                AuthorizedDate = s.AuthorizedDate,
                AuthorizedByUser = s.AuthorizedByUserId != null ? (s.AuthorizedByUser.FirstName + " " + s.AuthorizedByUser.MiddleName ?? "").Trim() + " " + s.AuthorizedByUser.Surname : "N/A",
                Status = s.Membership.IsActive == false && s.IsActive == false ? "Pending Authorization" :
s.Membership.IsActive && s.IsActive == true && !s.IsDeleted ? "Active" : s.Membership.IsActive == true && s.IsActive == false ? "Deactivated" :
s.Membership.IsDeleted == true ? "Denied" : s.IsDeleted == true ? "Deleted" : "N/A",
                RepresentativePostpaidClient = s.Membership.RepresentativeId != null ? ctx.Institutions.Where(b => b.Id == s.Membership.RepresentativeId).FirstOrDefault().Name : "N/A"


            });

            if (request.IsReportRequest)
            {
                query2 = query2.OrderByDescending(s => s.CreatedOn);
            }
            else
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }

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

                if (request.SortColumn == "Phone")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Phone);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Phone);
                    }
                }

                if (request.SortColumn == "Email")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Email);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Email);
                    }
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

            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.InstitutionGridView = query2.ToList();
            return response;
        }

        private IQueryable<Institution> InstitutionListHelper(ViewClientInstitutionsRequest request)
        {
            IQueryable<Institution> query = ctx.Institutions.AsNoTracking();

            if (!(String.IsNullOrEmpty(request.CompanyName)))
            {
                query = query.Where(s => s.Name.ToLower().Contains(request.CompanyName.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Email)))
            {
                query = query.Where(s => s.Address.Email.ToLower().StartsWith(request.Email.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Phone)))
            {
                query = query.Where(s => s.Address.Phone.ToLower().StartsWith(request.Phone.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.ClientCode)))
            {
                query = query.Where(s => s.Membership.ClientCode.ToLower().Contains(request.ClientCode.ToLower()));
            }
            if (request.MembershipGroupId > 0)
            {
                query = query.Where(s => s.Membership.MembershipGroup == request.MembershipGroupId);
            }

            

            if (!(request.MembershipStatus == null))
            {
                switch (request.MembershipStatus)
                {
                    case 0:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted);
                        break;
                    case 1:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && !s.IsActive && !s.IsDeleted);
                        break;
                    case 2:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && (s.IsActive || !s.IsActive) && s.IsDeleted);
                        break;
                    case 3:
                        query = query.Where(s => !s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted);
                        break;
                    case 4:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted || (!s.IsActive && !s.IsDeleted));
                        break;
                    case 5:
                        query = query.Where(s => !s.Membership.IsActive && s.Membership.IsDeleted && s.IsActive && s.IsDeleted);
                        break;
                }

            }

            /*if (request.SecuredPartyType != null)
            {
                query = query.Where(s => s.SecuringPartyTypeId == request.SecuredPartyType);
            }*/

            if (request.CreatedOn != null)
            {

                query = query.Where(s => DbFunctions.DiffDays(s.CreatedOn, request.CreatedOn) == 0);



            }

            //if (!(String.IsNullOrEmpty(request.CreatedRange)))
            if (request.CreatedRange != null)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }
            query = query.Where(s => s.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Client);

            return query;
        }



        public IQueryable<Institution> GetDbSetComplete()
        {

            return ctx.Institutions.Include("People").Include("InstitutionUnits")
                .Include("Membership");
            //.Include("FinancialStatementActivities.FinancialStatements");
        }

        //Also get all Institutions that have an account
    }



}
