using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;


namespace CRL.UI.MVC.Common
{
    public static class SelectListExtensions
    {
        //public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        //    where TEnum : struct, IComparable, IFormattable, IConvertible
        //{
        //    var values = from TEnum e in Enum.GetValues(typeof(TEnum))
        //                 select new { Id = e, Name = e.ToString() };
        //    return new SelectList(values, "Id", "Name", enumObj);
        //}
        public static SelectList ToSelectList(this IEnumerable<LookUpView> lkup)
        {
            return new SelectList(lkup, "LkValue", "LkName");
        }

        public static List<SelectListItem> ToSelectListItem(this IEnumerable<LookUpView> lkup)
        {

            List<SelectListItem> MajorityOwnershipList = new List<SelectListItem>();
            foreach (var a in lkup)
            {
                MajorityOwnershipList.Add(new SelectListItem { Text = a.LkName, Value = a.LkValue.ToString() });
            }

            return MajorityOwnershipList;


        }
        public static Dictionary<string, string> ToDictionaryString(this IEnumerable<LookUpView> lkup)
        {
            Dictionary<string, string> _categories = new Dictionary<string, string>();
            foreach (var item in lkup)
            {
                _categories.Add(item.LkValue.ToString(), item.LkName);
            }
            return _categories;
        }
    }
}