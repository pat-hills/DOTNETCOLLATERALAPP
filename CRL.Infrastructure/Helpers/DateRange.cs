using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Helpers
{
    public class DateRange
    {
        public DateRange()
        {

        }
        public DateRange(DateTime _startDate)
        {
            StartDate = _startDate.Date;
            EndDate = StartDate.AddDays(1).Date ;
        }

        public DateRange(DateTime _startDate, DateTime _endDate)
        {
            StartDate = _startDate.Date;
            EndDate = _endDate.AddDays(1).Date;
        }

        public DateRange(DateTime _startDate, int numDaysInterval)
        {
            StartDate = _startDate.Date;
            EndDate = StartDate.AddDays(numDaysInterval);
        }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string GenerateRangePhrase(string reportTitle=null)
        {
            string Phrase=null;
            if ((StartDate != null && EndDate == null) || (StartDate == null && EndDate != null))
            {
                Phrase = reportTitle + "On " + StartDate == null ? EndDate.AddDays(-1).ToShortDateString() : StartDate.ToShortDateString();
            }
            else if (StartDate != null && EndDate != null)
            {
                Phrase = reportTitle+ "From " + StartDate.ToShortDateString() + " to " + EndDate.AddDays(-1).ToShortDateString();
            }

            return Phrase;
        }


    }
}
