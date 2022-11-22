using System;
using System.Collections.Generic;
using System.Linq;
using BookingRentals.Calendar;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        protected readonly ICalendar _calendarService;
        protected readonly BookingRentals.Rental.IRental _rentalService;
        public CalendarController(
            ICalendar calendarService,
            BookingRentals.Rental.IRental rentalService)
        {
            _calendarService = calendarService;
            _rentalService = rentalService;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            try
            {
                var rentalItem = _rentalService.GetRentalById(rentalId);
                var calendarItems = _calendarService.GetCalendar(rentalItem, start, nights)
                    .GroupBy(x => x.Date, (key,grp) => new { Date = key, Bookings = grp.Where(x => x.BookingId != null).Select(x => new CalendarBookingViewModel() {Id = x.BookingId.Value }) });
                return new CalendarViewModel(){
                    RentalId = rentalId,
                    Dates = calendarItems.Select(x => new CalendarDateViewModel(){
                        Date = x.Date,
                        Bookings = x.Bookings.ToList()
                    }).OrderBy(x => x.Date).ToList()
                };
                
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
