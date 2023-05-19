using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Search;
using CRL.Model.Search.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Messaging;
using CRL.Infrastructure.Helpers;
using LinqKit;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;
using System.Data.Entity;
using CRL.Model.ModelViews.Enums;

namespace CRL.Repository.EF.All.Repository.Search
{




    public class SearchFinancialStatementRepository : Repository<SearchFinancialStatement, int>, ISearchFinancialStatementRepository
    {
        public SearchFinancialStatementRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "SearchFinancialStatement";
        }



        public ViewSearchesResponse SelectSearchesGridViewCQ(ViewSearchesFSRequest request)
        {

            ViewSearchesResponse response = new ViewSearchesResponse();
            var query = ctx.SearchFinancialStatements.AsNoTracking().Where(s => s.IsDeleted == false && s.IsActive == true);



            if (!(String.IsNullOrEmpty(request.SearchCode)))
            {
                query = query.Where(s => s.SearchCode.ToLower() == request.SearchCode.ToLower());
            }



            if (request.SearchDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.SearchDate.StartDate && s.CreatedOn < request.SearchDate.EndDate);
            }

            if (request.NewCreatedOn != null)
            {
                //query = query.Where(s => s.CreatedOn >= request.SearchDate.StartDate && s.CreatedOn < request.SearchDate.EndDate);

                query = query.Where(s => DbFunctions.DiffDays(s.CreatedOn, request.NewCreatedOn) == 0);

            }

            if (!(String.IsNullOrEmpty(request.UserName)))
            {
                query = query.Where(s => (s.CreatedByUser.FirstName.ToLower() + " " + (s.CreatedByUser.MiddleName + " " ?? "") + " " + s.CreatedByUser.Surname.ToLower()).Contains(request.UserName.ToLower())
                    || (s.CreatedByUser.FirstName + s.CreatedByUser.MiddleName + s.CreatedByUser.Surname).ToLower().Trim().Replace(" ", "") == (request.UserName.ToLower().Trim().Replace(" ", "")));
            }


            if (!(String.IsNullOrEmpty(request.ClientName)))
            {
                query = query.Where(s => s.CreatedByUser.Institution.Name.ToLower().StartsWith(request.ClientName.ToLower()));

            }
            if (request.LimitTo == true)
            {
                query = query.Where(s => s.CreatedByUser.Id == request.SecurityUser.Id);
            }
            if (request.IsReportRequest)
            {
                if (request.ClientType != null || request.ClientType > 0)
                {
                    if (request.ClientType.Value == 1)
                    {
                        query = query.Where(s => (s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2 && s.CreatedByUser.MembershipId == 1 && s.IsPublicUser == false));
                    }
                    else if (request.ClientType.Value == 2)
                    {
                        query = query.Where(s => s.CreatedByUser.Membership.MembershipTypeId == MembershipCategory.Client);
                    }
                    else if (request.ClientType.Value == 3)
                    {
                        query = query.Where(s => s.IsPublicUser == true);
                    }
                }
            }
            else
            {
                if (request.SecurityUser != null)
                {

                    if (request.SecurityUser.IsOwnerUser == false) //For client 
                    {
                        if (request.SecurityUser.IsClientAdministrator())
                        {
                            //Find out if we have to limit to 
                            if (request.LimitTo == true)
                                query = query.Where(s => s.CreatedByUser.Id == request.SecurityUser.Id);
                            else
                                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);



                        }
                        else
                        {
                            query = query.Where(s => s.CreatedByUser.Id == request.SecurityUser.Id);
                        }

                    }
                    else
                    {
                        if (request.ClientType != null || request.ClientType > 0)
                        {
                            if (request.ClientType.Value == 1)
                            {
                                query = query.Where(s => s.CreatedBy != 1);
                            }
                            else if (request.ClientType.Value == 2)
                            {
                                query = query.Where(s => s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2);
                            }
                            else if (request.ClientType.Value == 3)
                            {
                                query = query.Where(s => s.CreatedByUser.Membership.isIndividualOrLegalEntity == 1);
                            }
                            else if (request.ClientType.Value == 4)
                            {
                                query = query.Where(s => s.CreatedBy == 1);
                            }
                        }

                    }
                }
                else
                {
                    query = query.Where(s => s.PublicUsrCode == request.PublicUserCode);
                }
            }


            if (request.GeneratedReportType != null)
            {
                //if (request.GeneratedReportType == 1)
                //    query = query.Where(s => s.GeneratedUnCertifiedSearchReport == true || s.GeneratedCertifiedSearchReport  == true);
                //else if (request.GeneratedReportType == 2)
                //    query = query.Where(s => s.GeneratedCertifiedSearchReport == true);

            }
            if (request.SearchType != null)
            {

                query = query.Where(s => s.IsLegalOrFlexible == request.SearchType);

            }
            if (request.ReturnedResults != null)
            {
                if (request.ReturnedResults == 1)
                {
                    query = query.Where(s => s.NumRecords > 0);
                }
                else
                {
                    query = query.Where(s => s.NumRecords == 0);
                }


            }


            //if (request.InstitutionId != null && request.InstitutionId > 0)
            //{
            //    //Get the institution membership

            //    query = query.Where(s => s.Membership  == (CollateralCategory)request.CollateralTypeId);
            //}




            var query2 = query.Select(s => new SearchRequestGridView
            {
                Id = s.Id,
                ClientType = (s.CreatedBy == 1 ? "Registry User" : s.CreatedBy == 2 ? "Public User" :
                s.CreatedByUser.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                   (s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2 && s.CreatedByUser.MembershipId == 1) ? "Registry User" : (s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2) ? "Financial Institution User"
                    : "N/A"),
                IsLegalOrFlexible = s.IsLegalOrFlexible,
                PublicUsrCode = s.PublicUsrCode,
                IsPublicUser = s.IsPublicUser,
                SearchCode = s.SearchCode,
                NumRecords = s.NumRecords,
                SearchType = s.IsLegalOrFlexible == 1 ? "Legally Effective" : "Flexible",
                MembershipName = s.IsPublicUser == true ? "Public User" : (s.IsPublicUser == false && s.CreatedByUser.Institution != null) ? s.CreatedByUser.Institution.Name : "N/A",
                SearchRequestParametersString = s.SearchParamString,
                //MembershipName =
                //  (
                //  s.CreatedByUser .Membership .isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.CreatedByUser.MembershipId  && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                //  s.CreatedByUser .Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser .MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                //  ),
                MembershipId = s.CreatedByUser.MembershipId,
                SearchDate = s.CreatedOn,
                SearchRequestParameters = s.SearchParamXML,
                NameOfSearcher = (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName ?? "").Trim() + " " + s.CreatedByUser.Surname,
                InstitutionUnitId = s.InstitutionUnitId,
                InstitutionUnit = s.InstitutionUnit.Name != null ? s.InstitutionUnit.Name : "N/A",
                SearchReportId = s.GeneratedReportId,
                HasCertifiedReport = s.GeneratedReportId != null,
                LoginId = s.CreatedByUser.Username,
                FoundRegistrationNosString = s.FoundRegistrationString != "" ? s.FoundRegistrationString : "None",
                ReportGeneratedDate = s.GeneratedReport.CreatedOn
            });


            if ((String.IsNullOrWhiteSpace(request.SortColumn)))
            {
                request.SortColumn = "SearchDate";
                request.SortOrder = "desc";
            }

            if (request.SortColumn == "SearchDate")
            {
                if (request.SortOrder == "desc")
                {
                    query2 = query2.OrderByDescending(s => s.SearchDate);
                }
                else
                {
                    query2 = query2.OrderBy(s => s.SearchDate);
                }
            }

            if (request.SortColumn == "SearchCode")
            {
                if (request.SortOrder == "desc")
                {
                    query2 = query2.OrderByDescending(s => s.SearchCode);
                }
                else
                {
                    query2 = query2.OrderBy(s => s.SearchCode);
                }
            }



            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.SearchRequestGridView = query2.ToList();
            return response;


        }
        public string[] Search(SearchRequest request)
        {

            string BorrowerFlexibleName = "";
            if (!String.IsNullOrWhiteSpace(request.SearchParam.BorrowerName))
            {
                BorrowerFlexibleName = Util.GetFlexibleBorrowerNameForSearch(request.SearchParam.BorrowerName);
            }
            //Note we only need where the collateral is active and not discharged
            //FS should be active and not discharge
            //FS should not be terminated
            //Borrower and Lender should be active

            //HAVE MANY VERSIONS OF THE VIEW CLASS AND SHOULD BE CONFIGURABLE

            //first search is for using or so that we get or between collateral and serial
            //that is both by collateral and serial
            //then we try to make sure that they have the same registration o

            var outer = PredicateBuilder.False<Model.FS.FinancialStatement>();

            var param = request.SearchParam;
            var inner = PredicateBuilder.True<Model.FS.FinancialStatement>();

            //IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSetComplete();   
            if (param.SearchType == 1)
            {
                //if ((!String.IsNullOrWhiteSpace(param.BorrowerFirstName) && !String.IsNullOrWhiteSpace(param.BorrowerLastName)))
                //{
                //    inner = inner.And(s => s.Participants.OfType<IndividualParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower &&
                //   ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.Identification.CardNo.TrimEnd()))
                //   && ( param.BorrowerFirstName.Trim().ToLower() == t.Identification.FirstName.TrimEnd().ToLower())
                //   && (param.BorrowerMiddleName.Trim().ToLower() == t.Identification.MiddleName.TrimEnd().ToLower())
                //   && (param.BorrowerLastName.Trim().ToLower() == t.Identification.Surname.TrimEnd().ToLower())
                //   ));
                //}
                //else 
                if (!(String.IsNullOrWhiteSpace(param.BorrowerFirstName) && String.IsNullOrWhiteSpace(param.BorrowerLastName)
                    && String.IsNullOrWhiteSpace(param.BorrowerIDNo)))
                {
                    inner = inner.And(s => s.Participants.OfType<IndividualParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower && ((
                     ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.Identification.CardNo.TrimEnd()))
                     && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == t.Identification.FirstName.TrimEnd().ToLower()))
                     && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == t.Identification.MiddleName.TrimEnd().ToLower()))
                     && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == t.Identification.Surname.TrimEnd().ToLower()))

                     ) || (t.OtherPersonIdentifications.Any(u =>
                          ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == u.Identification.CardNo.TrimEnd()))
                            && ((String.IsNullOrEmpty(param.BorrowerFirstName) || param.BorrowerFirstName.Trim().ToLower() == u.Identification.FirstName.TrimEnd().ToLower()))
                     && ((String.IsNullOrEmpty(param.BorrowerMiddleName) || param.BorrowerMiddleName.Trim().ToLower() == u.Identification.MiddleName.TrimEnd().ToLower()))
                     && ((String.IsNullOrEmpty(param.BorrowerLastName) || param.BorrowerLastName.Trim().ToLower() == u.Identification.Surname.TrimEnd().ToLower()))
                          )

                     ))));
                }

            }
            else
            {
                if (!(String.IsNullOrWhiteSpace(param.BorrowerName) && String.IsNullOrWhiteSpace(param.BorrowerIDNo)))
                {
                    inner = inner.And(s => s.Participants.OfType<InstitutionParticipant>().Any(t => t.ParticipationTypeId == ParticipationCategory.AsBorrower &&
                   ((String.IsNullOrEmpty(param.BorrowerIDNo) || param.BorrowerIDNo == t.CompanyNo.TrimEnd() && param.BusinessPrefix == t.BusinessRegistrationPrefixId))

                   && ((String.IsNullOrEmpty(param.BorrowerName) || t.SearchableName == BorrowerFlexibleName))));
                }
            }

            if (!(String.IsNullOrEmpty(param.CollateralSerialNo)))
            {
                inner = inner.And(s => s.Collaterals.Any(c => c.SerialNo == param.CollateralSerialNo


                    ));

            }
            inner = inner.And(s => s.isApprovedOrDenied == 1 && s.IsActive == true && s.ClonedId == null);  //for now is active is true but ti should not be
            outer = outer.Or(inner.Expand());

            var query = ctx.FinancialStatements.AsExpandable().Where(outer);
            SearchParam param2 = request.SearchParam;


            //After we get disntinct then we need need to check that if both borrower and collateral were entered
            //then we need to make sure that we filter out those registra
            var query_array = query.ToArray().Select(s => s.RegistrationNo).Distinct().ToArray();
            return query_array;





        }

    }

}
