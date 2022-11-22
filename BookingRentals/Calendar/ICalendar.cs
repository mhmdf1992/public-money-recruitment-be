using System;
using System.Collections.Generic;
using System.Text;

namespace BookingRentals.Calendar
{
    public interface ICalendar
    {
        IEnumerable<int> GetUnitsAvailable(Rental.RentalItem rentalItem, DateTime start, int nights);
        IEnumerable<CalendarBookingItem> GetCalendar(Rental.RentalItem rentalItem, DateTime start, int nights);
        bool CheckUnitsChangeOverlap(int rentalId, IEnumerable<int> unitsRemoved);
        IEnumerable<string> AddBooking(Rental.RentalItem rentalItem, DateTime start, int nights, int unit, int bookingId);
        IEnumerable<string> ExtendPreparationDays(int rentalId, int days);
        IEnumerable<string> ShrinkPreparationDays(int rentalId, int days);
        bool CheckRentalHasBookings(int rentalId);
    }
}
