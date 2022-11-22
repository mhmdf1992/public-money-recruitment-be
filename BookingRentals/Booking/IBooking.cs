using System;
using System.Collections.Generic;
using System.Text;

namespace BookingRentals.Booking
{
    public interface IBooking
    {
        BookingItem GetBookingById(int id);
        int AddBooking(int rentalId, System.DateTime start, int nights);
    }
}
