using BookingRentals.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Rental;

namespace VacationRental.Calendar
{
    public class CalendarService : ICalendar, ICalendarEngine
    {
        protected readonly Repository.IRepository<IdentifiedCalendarBookingItem> _calendarRepository;
        public CalendarService(Repository.IRepository<IdentifiedCalendarBookingItem> calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        public IEnumerable<CalendarBookingItem> GetCalendar(BookingRentals.Rental.RentalItem rentalItem, DateTime start, int nights)
        {
            try
            {
                var keys = GenerateCalendarBookingItems(
                    startDate: start, 
                    rentalId: rentalItem.Id, 
                    nights: nights, 
                    preparationDays: rentalItem.PreparationTimeInDays,
                    units: Enumerable.Range(1, rentalItem.Units)).Select(x => x.Key);
                return _calendarRepository.Get(x => x.RentalId == rentalItem.Id && x.Date >= start && x.Date <= start.AddDays(nights + rentalItem.PreparationTimeInDays))
                    .Join(keys, x => x.Key, y => y, (x, y) => x);
            }
            catch { throw; }
        }

        public IEnumerable<int> GetUnitsAvailable(BookingRentals.Rental.RentalItem rentalItem, DateTime start, int nights)
        {
            try
            {
                var allItems = GenerateCalendarBookingItems(
                    startDate: start, 
                    rentalId: rentalItem.Id, 
                    nights: nights, 
                    preparationDays: rentalItem.PreparationTimeInDays, 
                    units: Enumerable.Range(1, rentalItem.Units));
                var calendarItems = _calendarRepository.Get(x => x.RentalId == rentalItem.Id && x.Date >= start && x.Date <= start.AddDays(nights + rentalItem.PreparationTimeInDays));
                return allItems.Select(x => x.Key)
                    .Except(calendarItems.Select(x => x.Key))
                        .Join(allItems, x => x, y => y.Key, (x,y) => y)
                            .Select(x => x.Unit)
                                .Distinct();
            }
            catch { throw; }
        }

        public IEnumerable<string> AddBooking(BookingRentals.Rental.RentalItem rentalItem, DateTime start, int nights, int unit, int bookingId)
        {
            try
            {
                var items = GenerateCalendarBookingItems(
                        startDate: start,
                        rentalId: rentalItem.Id,
                        nights: nights,
                        preparationDays: rentalItem.PreparationTimeInDays,
                        units: new int[] { unit },
                        bookingId: bookingId);
                _calendarRepository.InsertRange(items);
                return items.Select(x => x.Key);
            }
            catch { throw; }
        }

        public bool CheckUnitsChangeOverlap(int rentalId, IEnumerable<int> unitsRemoved) => _calendarRepository.Any(x => x.RentalId == rentalId && unitsRemoved.Contains(x.Unit));

        public IEnumerable<IdentifiedCalendarBookingItem> GetPreparationDaysToAdd(int rentalId, int days)
            => _calendarRepository
                .Get(x => x.RentalId == rentalId && x.BookingId == null)
                .GroupBy(x => x.PreparationAfterBookingId, (key, grp) => grp.OrderByDescending(x => x.Date).First())
                .SelectMany(lstPrepDay =>
                    Enumerable.Range(1, days)
                        .Select(x => new IdentifiedCalendarBookingItem()
                        {
                            BookingId = lstPrepDay.BookingId,
                            Date = lstPrepDay.Date.AddDays(x),
                            Id = $"{lstPrepDay.Date.AddDays(x):yyyy-MM-dd}|{lstPrepDay.RentalId}|{lstPrepDay.Unit}",
                            PreparationAfterBookingId = lstPrepDay.PreparationAfterBookingId,
                            RentalId = lstPrepDay.RentalId,
                            Unit = lstPrepDay.Unit
                        }));

        public IEnumerable<string> GetPreparationDaysKeysToRemove(int rentalId, int days)
            => _calendarRepository
                .Get(x => x.RentalId == rentalId && x.BookingId == null)
                .GroupBy(x => x.PreparationAfterBookingId, (key, grp) => new { ItemsToRemove = grp.OrderByDescending(x => x.Date).Take(days).Select(x => x.Key) })
                .SelectMany(res => res.ItemsToRemove);

        public IEnumerable<IdentifiedCalendarBookingItem> GenerateCalendarBookingItems(DateTime startDate, int rentalId, int nights, int preparationDays, IEnumerable<int> units, int? bookingId = null)
            => Enumerable.Range(0, nights + preparationDays)
                .SelectMany(day =>
                    units.Select(unit => new IdentifiedCalendarBookingItem(){
                            Id = $"{startDate.AddDays(day):yyyy-MM-dd}|{rentalId}|{unit}",
                            Date = startDate.AddDays(day),
                            RentalId = rentalId,
                            Unit = unit,
                            BookingId = day < nights ? bookingId : null,
                            PreparationAfterBookingId = day >= nights ? bookingId : null
                    }));

        public IEnumerable<string> ExtendPreparationDays(int rentalId, int days)
        {
            var itemsToAdd = GetPreparationDaysToAdd(rentalId, days);
            var reserved = _calendarRepository.Get(x => x.RentalId == rentalId).Join(itemsToAdd, x => x.Id, y => y.Id, (x, y) => x);
            if (reserved != null && reserved.Any())
                throw new Exception("Day is already reserved for different booking");
            _calendarRepository.InsertRange(itemsToAdd);
            return itemsToAdd.Select(x => x.Key);
        }

        public IEnumerable<string> ShrinkPreparationDays(int rentalId, int days)
        {
            var itemsToRemove = GetPreparationDaysKeysToRemove(rentalId, days);
            _calendarRepository.DeleteRange(itemsToRemove);
            return itemsToRemove;
        }

        public bool CheckRentalHasBookings(int rentalId) => _calendarRepository.Any(x => x.RentalId == rentalId);
    }
}
