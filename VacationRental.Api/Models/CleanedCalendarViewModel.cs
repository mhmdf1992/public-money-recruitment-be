using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CleanedCalendarViewModel : CalendarViewModel
    {
        public new List<CleanedCalendarDateViewModel> Dates { get; set; }
    }
}
