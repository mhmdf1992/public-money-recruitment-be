using System;
using System.Collections.Generic;
using System.Linq;
using BookingRentals.Calendar;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/vacationrental/calendar")]
    [ApiController]
    public class VacationRentalCalendarController : ControllerBase
    {
        protected readonly ICalendar _calendarService;
        protected readonly BookingRentals.Rental.IRental _rentalService;
        public VacationRentalCalendarController(
            ICalendar calendarService,
            BookingRentals.Rental.IRental rentalService)
        {
            _calendarService = calendarService;
            _rentalService = rentalService;
        }

        [HttpGet]
        public CleanedCalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            try
            {
                var calendarItems = _calendarService.GetCalendar(_rentalService.GetRentalById(rentalId), start, nights)
                    .GroupBy(x => x.Date, (key,grp) => new { Date = key, Bookings = grp.Select(x => new CalendarUnitBookingViewModel() {Id = x.BookingId ?? 0, Unit = x.Unit }) });
                return new CleanedCalendarViewModel(){
                    RentalId = rentalId,
                    Dates = calendarItems.Select(x => new CleanedCalendarDateViewModel()
                    {
                        Date = x.Date,
                        Bookings = x.Bookings.Where(b => b.Id != 0).ToList(),
                        PreparationTimes = x.Bookings.Where(b => b.Id == 0).Select(b => new CalendarPreparationTimeViewModel() { Unit = b.Unit}).ToList()
                    }).OrderBy(x => x.Date).ToList()
                };
                
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
