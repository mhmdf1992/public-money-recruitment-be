using System;
using System.Collections.Generic;
using System.Linq;
using BookingRentals.Booking;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        protected readonly IBooking _bookingService;
        protected readonly BookingRentals.Calendar.ICalendar _calendarService;
        protected readonly BookingRentals.Rental.IRental _rentalService;

        public BookingsController(
            IBooking bookingService,
            BookingRentals.Calendar.ICalendar calendarService,
            BookingRentals.Rental.IRental rentalService)
        {
            _bookingService = bookingService;
            _calendarService = calendarService;
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            try
            {
                var bookingItem = _bookingService.GetBookingById(bookingId);
                return new BookingViewModel(){
                    Id = bookingItem.Id,
                    Nights = bookingItem.Nights,
                    RentalId = bookingItem.RentalId,
                    Start = bookingItem.Start
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            try
            {
                var rentalItem = _rentalService.GetRentalById(model.RentalId);
                var unitsAvailable = _calendarService.GetUnitsAvailable(rentalItem, model.Start, model.Nights);
                if (unitsAvailable == null || !unitsAvailable.Any())
                    throw new ArgumentException($"Rental {model.RentalId} not available", nameof(model.RentalId));
                var bookingId = _bookingService.AddBooking(model.RentalId, model.Start, model.Nights);
                _calendarService.AddBooking(rentalItem, model.Start, model.Nights, unitsAvailable.First(), bookingId);
                return new ResourceIdViewModel() { 
                    Id = bookingId
                };
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
