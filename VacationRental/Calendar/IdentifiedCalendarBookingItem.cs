using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Calendar
{
    public class IdentifiedCalendarBookingItem : BookingRentals.Calendar.CalendarBookingItem, Repository.Identified
    {
        public string Key { get => Id; }
    }
}
