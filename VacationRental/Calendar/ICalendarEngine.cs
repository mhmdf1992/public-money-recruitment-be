using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Calendar
{
    public interface ICalendarEngine
    {
        IEnumerable<IdentifiedCalendarBookingItem> GetPreparationDaysToAdd(int rentalId, int days);
        IEnumerable<string> GetPreparationDaysKeysToRemove(int rentalId, int days);
        IEnumerable<IdentifiedCalendarBookingItem> GenerateCalendarBookingItems(DateTime startDate, int rentalId, int nights, int preparationDays, IEnumerable<int> units, int? bookingId = null);
    }
}
