using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.ModelViews.Statistics
{
    public class CountOfItemStatView
    {
        //public int Id { get; set; }
        //public DateTime RegistrationDate{get;set;}
        public int MonthNum { get; set; }
        public int Year { get; set; }
        public int CountOfItem { get; set; }
        //public string Currency { get; set; }
        //public decimal SumOfLoanAmount { get; set; }
        public string GroupingColumn { get; set; }
        public string PrimaryGroup { get; set; }
    }

    public class FSCustomQueryStatView
    {
        public int MonthNum { get; set; }
        public int Year { get; set; }
        public int CountOfItem { get; set; }
        public string TransactionType { get; set; }
        public string LoanType { get; set; }
        public string Currency { get; set; }
        public decimal SumOfLoanAmount { get; set; }
        public string GroupingColumn { get; set; }

    }
    public class ValueOfFSStatView
    {

        public int MonthNum { get; set; }
        public int Year { get; set; }
        public string Currency { get; set; }
        public decimal SumOfLoanAmount { get; set; }
        public string GroupingColumn { get; set; }


    }

    public class ValueOfFeeStatView
    {

        public int MonthNum { get; set; }
        public int Year { get; set; }
        public decimal SumOfLoanAmount { get; set; }
        public string GroupingColumn { get; set; }
        public DateRange TransactionDate { get; set; }
        public string PrimaryGroup { get; set; }

    }


    public class ValueAndCountOfFSStatView
    {
        public int MonthNum { get; set; }
        public int Year { get; set; }
        public decimal SumOfLoanAmount { get; set; }
        public int CountOfLoanAmountRange { get; set; }
        public string GroupingColumn { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }



    }


    public class VariableLoanRange
    {
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string RangeString { get { return this.GenerateRangeString(); } }
        public string GenerateRangeString()
        {

            string rangeString = this.Min != null ? this.Min.ToString() + " - " + this.Max.ToString()
                : "< " + this.Max.ToString();
            return rangeString;
        }
    }
}
