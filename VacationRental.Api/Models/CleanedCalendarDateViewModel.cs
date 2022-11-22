using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CleanedCalendarDateViewModel : CalendarDateViewModel
    {
        public new List<CalendarUnitBookingViewModel> Bookings { get; set; }
        public List<CalendarPreparationTimeViewModel> PreparationTimes { get; set; }
    }
}
