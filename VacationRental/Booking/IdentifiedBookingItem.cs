using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Booking
{
    public class IdentifiedBookingItem : BookingRentals.Booking.BookingItem, Repository.Identified
    {
        public string Key { get => Id.ToString(); }
    }
}
