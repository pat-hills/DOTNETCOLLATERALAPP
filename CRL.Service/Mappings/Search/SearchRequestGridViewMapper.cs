using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;

using CRL.Model.Search;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;


namespace CRL.Service.Mappings.Search
{
    public static class SearchRequestGridViewMapper
    {
        public static ICollection<SearchRequestGridView> ConvertToSearchRequestGridView(this IEnumerable<SearchFinancialStatement> model)
        {
            ICollection<SearchRequestGridView> iview = new List<SearchRequestGridView>();
            foreach (var s in model)
            {
                iview.Add(s.ConvertToFSGridView());
            }

            return iview;

        }

        public static SearchRequestGridView ConvertToFSGridView(this SearchFinancialStatement model)
        {
            SearchRequestGridView iview = new SearchRequestGridView();
            iview.Id = model.Id;
            iview.SearchDate = model.CreatedOn;
            iview.SearchCode = model.SearchCode;
            //iview.HasCertifiedReport = model.SearchRequestCertifiedResultId != null;
            iview.HasUncertifiedReport = model.GeneratedReportId  != null;
            iview.MembershipId = model.CreatedByUser .MembershipId ; //If public user then created by will be a specific anonymous user representative in the system
            iview.NameOfSearcher = NameHelper.GetFullName(model.CreatedByUser.FirstName, model.CreatedByUser.MiddleName, model.CreatedByUser.Surname);
            if (model.CreatedByUser.InstitutionId!= null)
            {
                    iview.MembershipName   = model.CreatedByUser.Institution .Name ;
               
            }
            iview.IsPublicUser = model.CreatedBy  == 1;//For public user

            return iview;

        }
       
    }
}
