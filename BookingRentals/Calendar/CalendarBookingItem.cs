using System;
using System.Collections.Generic;
using System.Text;

namespace BookingRentals.Calendar
{
    public class CalendarBookingItem
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int RentalId { get; set; }
        public int Unit { get; set; }
        public int? BookingId { get; set; }
        public int? PreparationAfterBookingId { get; set; }
    }
}
