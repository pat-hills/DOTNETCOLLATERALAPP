using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;

namespace CRL.UI.MVC.HTMLHelpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues, bool Inline=false)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers
                    //var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    string classattr = "radio";
                    if (Inline)
                    {
                        classattr += " inline";
                    }
                    if (item.Selected)
                    {
                        var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id , Checked = "checked"}).ToHtmlString();
                        sb.AppendFormat("<label class=\"" + classattr + "\">{0}{1}</label>", radio, item.Text );
                    }
                    else
                    {
                        var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();
                        sb.AppendFormat("<label class=\"" + classattr + "\">{0}{1}</label>", radio, item.Text );
                    }
                    var selected = item.Selected = true;

                    // Create the html string that will be returned to the client
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label>
                   
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}