using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Helpers;

namespace CRL.UI.MVC.Common
{
    public class BaseSearchFilterViewModel : BaseDetailViewModel
    {
        public BaseSearchFilterViewModel()
        {
            List<SelectListItem> month_items = new List<SelectListItem>();
            List<SelectListItem> year_items = new List<SelectListItem>();
            ReportTypes = new List<SelectListItem>();
            month_items.Add(new SelectListItem { Text = "All", Value = "0", Selected = false });
            DateTime month = Convert.ToDateTime("1/1/2000");
            for (int i = 2013; i < 2031; i++)
            {
                SelectListItem it = new SelectListItem();
                it.Text = i.ToString();
                it.Value = i.ToString();
                string Yr = DateTime.Now.Year.ToString();
                if (it.Value == Yr)
                    it.Selected = true;
                year_items.Add(it);
            }


            for (int i = 0; i < 12; i++)
            {
                DateTime nextMonth = month.AddMonths(i);
                SelectListItem it = new SelectListItem();
                it.Text = nextMonth.ToString("MMMM");
                it.Value = nextMonth.Month.ToString();
                if (it.Value == DateTime.Now.Month.ToString())
                    it.Selected = true;
                month_items.Add(it);
            }

            MonthsList = month_items;
            YearList = year_items;
            UseStartEndDateOption = true;

            ReportTypes.Add(new SelectListItem { Text = "Detailed Report", Value = "1" });
            ReportTypes.Add(new SelectListItem { Text = "Summary Report", Value = "2" });
        }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        //[RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid start date provided")]
        public Nullable<DateTime> StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        //[RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid end date provided")]
        public Nullable<DateTime> EndDate { get; set; }
        public int SelectedMonthList { get; set; }
        
        [Display(Name = "Report Type")]
        public int? ReportTypeId { get; set; }
        public int Status { get; set; }
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid selected year")]
        public string SelectedYear { get; set; }
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Start/End Date option provided")]
        public bool UseStartEndDateOption { get; set; }
        public IEnumerable<SelectListItem> MonthsList { get; set; }
        public IEnumerable<SelectListItem> YearList { get; set; }
        
        public ICollection<SelectListItem> ReportTypes { get; set; }  

        public DateRange GenerateDateRange()
        {

            if (this.UseStartEndDateOption == false)
            {


                int year = Convert.ToInt32(this.SelectedYear);
                if (year != 0)
                {

                    int month = this.SelectedMonthList;
                    DateTime c1; DateTime c2;
                    if (month == 0)
                    {
                        c1 = new DateTime(year, 1, 1);
                        c2 = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
                    }
                    else
                    {
                        c1 = new DateTime(year, month, 1);
                        c2 = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    }
                    DateRange _dateRange = new DateRange(c1, c2);
                    //_dateRange.StartDate = c1;
                    //_dateRange.EndDate = c2;
                    return _dateRange;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (this.StartDate != null && this.EndDate != null)
                {
                    DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.StartDate, (DateTime)this.EndDate);
                    return _dateRange;
                }
                else if (this.StartDate != null && this.EndDate == null)
                {
                    DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.StartDate);
                    return _dateRange;
                }
                else if (this.StartDate == null && this.EndDate != null)
                {
                    DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.EndDate);
                    return _dateRange;
                }

            }


            return null;
        }


    }
}