using BookingRentals.Booking;
using BookingRentals.Calendar;
using BookingRentals.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VacationRental.Rental;

namespace VacationRental.Booking
{
    public class BookingService : BookingRentals.Booking.IBooking, IBookingEngine
    {
        protected readonly Repository.IRepository<IdentifiedBookingItem> _bookingsRepository;
        public BookingService(Repository.IRepository<IdentifiedBookingItem> bookingsRepository)
        {
            _bookingsRepository = bookingsRepository;
        }

        public int AddBooking(int rentalId, DateTime start, int nights)
        {
            if (nights <= 0)
                throw new ArgumentException($"{nights} is not valid {nameof(nights)}", nameof(nights));
            try
            {
                int.TryParse(_bookingsRepository.Insert(new IdentifiedBookingItem()
                {
                    Id = GenerateId(),
                    RentalId = rentalId,
                    Start = start,
                    Nights = nights
                }), out int retId);
                return retId;
            }
            catch { throw; }
        }

        public int GenerateId() => (int)_bookingsRepository.Count() + 1;

        public BookingItem GetBookingById(int id)
        {
            try
            {
                return _bookingsRepository.Get(id.ToString());
            }
            catch { throw; }
        }
    }
}
