using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.HTMLHelpers
{
    public static class PageLinksHelper
    {
        public static void BuildLinks(int start, int end, string innerContent, int CurrentPage, TagBuilder divTag)
        {

            //<div class="dataTables_paginate paging_full_numbers" id="DataTables_Table_8_paginate">
            //<a tabindex="0" class="first paginate_button paginate_button_disabled" id="DataTables_Table_8_first">First</a>
            //<a tabindex="0" class="previous paginate_button paginate_button_disabled" id="DataTables_Table_8_previous">Previous</a>
            //<span><a tabindex="0" class="paginate_active">1</a><a tabindex="0" class="paginate_button">2</a><a tabindex="0" class="paginate_button">3</a></span>
            //<a tabindex="0" class="next paginate_button" id="DataTables_Table_8_next">Next</a><a tabindex="0" class="last paginate_button" id="DataTables_Table_8_last">Last</a></div>

            for (int i = start; i <= end; i++)
            {
                 TagBuilder aTag = new TagBuilder("a");
                 aTag.AddCssClass("saveButton");
                 
                 if (i == CurrentPage) aTag.AddCssClass("paginate_active"); else aTag.AddCssClass("paginate_button");
                aTag.SetInnerText(innerContent ?? i.ToString());
                UrlHelper urlhelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                string url = urlhelper.Action("Index", "Home", new { page=i});
                aTag.MergeAttribute("href", url);
                aTag.MergeAttribute("data-page", i.ToString ());
                aTag.MergeAttribute("tabindex", "0");
                divTag.InnerHtml += aTag.ToString();
                //<a  href="@Url.Action("products", "home", new { page = i })">@(innerContent ?? i.ToString())</a>  
            }
          
                

        }

        public static MvcHtmlString PageLinks(this HtmlHelper html, int TotalPages, int CurrentPage)
        {
            const int maxPages = 11;
       
        
            TagBuilder spanTag = new TagBuilder("span");

            if (TotalPages <= maxPages)
            {
                BuildLinks(1, TotalPages, null, CurrentPage, spanTag);              
                return new MvcHtmlString(spanTag.ToString());


            }

            int pagesAfter = TotalPages - CurrentPage; // Number of pages after current
            int pagesBefore = CurrentPage - 1; // Number of pages before current

            if (pagesAfter <= 4)
            {

                BuildLinks(1, 1, null, CurrentPage, spanTag); // Show 1st page

                int pageSubset = TotalPages - maxPages - 1 > 1 ? TotalPages - maxPages - 1 : 2;
                BuildLinks(pageSubset, pageSubset, "...", CurrentPage, spanTag); // Show page subset (...)

                BuildLinks(TotalPages - maxPages + 3, TotalPages, null, CurrentPage, spanTag);// Show last pages
                return new MvcHtmlString(spanTag.ToString()); // Exit
            }

            if (pagesBefore <= 4)
            {
                BuildLinks(1, maxPages - 2, null, CurrentPage, spanTag); // Show 1st pages


                int pageSubset = maxPages + 2 < TotalPages ? maxPages + 2 : TotalPages - 1;
                BuildLinks(pageSubset, pageSubset, "...", CurrentPage, spanTag); // Show page subset (...)

                BuildLinks(TotalPages, TotalPages, null, CurrentPage, spanTag); // Show last page               
                return new MvcHtmlString(spanTag.ToString()); // Exit

            }

            if (pagesAfter > 4)
            {
                BuildLinks(1, 1, null, CurrentPage, spanTag); // Show 1st pages

                int pageSubset1 = CurrentPage - 7 > 1 ? CurrentPage - 7 : 2;
                int pageSubset2 = CurrentPage + 7 < TotalPages ? CurrentPage + 7 : TotalPages - 1;

                BuildLinks(pageSubset1, pageSubset1, pageSubset1 == CurrentPage - 4 ? null : "...", CurrentPage, spanTag); // Show 1st page subset (...)

                BuildLinks(CurrentPage - 3, CurrentPage + 3, null, CurrentPage, spanTag);// Show middle pages

                // Show 2nd page subset (...)
                // only show ... if page is contigous to the previous one.
                BuildLinks(pageSubset2, pageSubset2, pageSubset2 == CurrentPage + 4 ? null : "...", CurrentPage, spanTag);
                BuildLinks(TotalPages, TotalPages, null, CurrentPage, spanTag);// Show last page
                return new MvcHtmlString(spanTag.ToString()); // Exit
            }

            return null;
        }

    }
}