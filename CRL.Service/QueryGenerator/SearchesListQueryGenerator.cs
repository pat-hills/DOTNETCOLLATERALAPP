using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;
using CRL.Model.Search;
using CRL.Model.Search.IRepository;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Search;
using CRL.Model.Search;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Search;


namespace CRL.Service.QueryGenerator
{
    public static class  SearchesListQueryGenerator
    {
        public static IQueryable<SearchRequestGridView> CreateQueryForFindFS(
          ViewSearchesFSRequest request, ISearchFinancialStatementRepository _rpFS, bool DoCount)
        {

            CBLContext ctx = ((SearchFinancialStatementRepository)_rpFS).ctx;
            IQueryable<SearchFinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true);

            if (!(String.IsNullOrEmpty(request.SearchCode )))
            {
                query = query.Where(s => s.SearchCode .ToLower() == request.SearchCode .ToLower());
            }

         

            if (request.SearchDate != null)
            {
                query = query.Where(s => s.CreatedOn  >= request.SearchDate.StartDate && s.CreatedOn < request.SearchDate.EndDate);
            }

            if (!(String.IsNullOrEmpty(request.ClientName)))
            {
                query = query.Where(s => s.CreatedByUser.Institution.Name.ToLower().StartsWith(request.ClientName.ToLower()));

            }
            if (request.SecurityUser != null)
            {

                if (request.SecurityUser.IsOwnerUser == false) //For client 
                {
                    if (request.SecurityUser.IsClientAdministrator())
                    {
                        //Find out if we have to limit to 
                        if (request.LimitTo ==true)                            
                                 query = query.Where(s => s.CreatedByUser.Id  == request.SecurityUser.Id );
                            else
                                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);

                        
                       
                    }
                    else
                    {
                        query = query.Where(s => s.CreatedByUser.Id  == request.SecurityUser.Id );
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

            
            if (request.GeneratedReportType  != null)
            {
                //if (request.GeneratedReportType == 1)
                //    query = query.Where(s => s.GeneratedUnCertifiedSearchReport == true || s.GeneratedCertifiedSearchReport  == true);
                //else if (request.GeneratedReportType == 2)
                //    query = query.Where(s => s.GeneratedCertifiedSearchReport == true);

            }
            if (request.SearchType  != null)
            {
              
                    query = query.Where(s => s.IsLegalOrFlexible == request.SearchType);
             
            }
            if (request.ReturnedResults  != null)
            {
                if (request.ReturnedResults == 1)
                {
                    query = query.Where(s => s.NumRecords >0);
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
                 ClientType = (s.CreatedBy ==1 ?"Public User" : s.CreatedByUser.Membership.isIndividualOrLegalEntity ==1? "Individual" : "Financial Institution"),
                 //FoundRegistrationNos = s.FoundRegistrationNos ,
                 // HasCertifiedReport = s.GeneratedCertifiedSearchReport ==true,
                 // HasUncertifiedReport = s.GeneratedUnCertifiedSearchReport ==true,
                   IsLegalOrFlexible = s.IsLegalOrFlexible ,
                    PublicUsrCode = s.PublicUsrCode ,
                     IsPublicUser = s.IsPublicUser ,
                      SearchCode = s.SearchCode ,
                       NumRecords = s.NumRecords ,
                         SearchRequestResultId =s.GeneratedReportId  ,
                       //SearchRequestCertifiedResultId  =s.SearchRequestCertifiedResultId ,
                       SearchType = s.IsLegalOrFlexible ==1 ? "Legally Effective" : "Flexible",
                MembershipName = s.CreatedByUser.Institution != null ? s.CreatedByUser.Institution.Name : "N/A",
                 //FoundRegistrationNosString = s.FoundRegistrationNosString ,
                  SearchRequestParametersString =s.SearchParamString ,
                        //MembershipName =
                        //  (
                        //  s.CreatedByUser .Membership .isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.CreatedByUser.MembershipId  && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                        //  s.CreatedByUser .Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser .MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                        //  ),
                MembershipId = s.CreatedByUser.MembershipId ,
                 SearchDate = s.CreatedOn ,
                  SearchRequestParameters = s.SearchParamXML ,
                   NameOfSearcher =(s.CreatedByUser.FirstName + " "+ s.CreatedByUser.MiddleName ?? "").Trim () + " " + s.CreatedByUser.Surname
            });

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "SearchDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "SearchDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.SearchDate );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.SearchDate  );
                    }
                }

                if (request.SortColumn == "SearchCode")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.SearchCode );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.SearchCode);
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
