using System;
using System.Collections.Generic;
using System.Linq;
using BookingRentals.Rental;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        protected readonly IRental _rentalService;
        protected readonly BookingRentals.Calendar.ICalendar _calendarService;

        public RentalsController(IRental rentalService, BookingRentals.Calendar.ICalendar calendarService)
        {
            _rentalService = rentalService;
            _calendarService = calendarService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            try
            {
                var rentalItem = _rentalService.GetRentalById(rentalId);
                return new RentalViewModel(){
                    Id = rentalItem.Id,
                    Units = rentalItem.Units
                };
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            try
            {
                return new ResourceIdViewModel() { 
                    Id = _rentalService.AddRental(model.Units)
                };
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public ResourceIdViewModel Put([FromRoute] int rentalId, [FromBody] CleanedRentalBindingModel model)
        {
            try
            {
                var rentalItem = _rentalService.GetRentalById(rentalId);
                var rentalHasBookings = _calendarService.CheckRentalHasBookings(rentalId);
                if (model.Units < rentalItem.Units && rentalHasBookings)
                {
                    if (_calendarService.CheckUnitsChangeOverlap(rentalId, Enumerable.Range(model.Units + 1, rentalItem.Units - model.Units)))
                        throw new ArgumentException($"Unit is already reserved", nameof(model.Units));
                }
                if (rentalItem.PreparationTimeInDays != model.PreparationTimeInDays && rentalHasBookings)
                {
                    var changedKeys = rentalItem.PreparationTimeInDays > model.PreparationTimeInDays
                                            ? _calendarService.ShrinkPreparationDays(rentalId, rentalItem.PreparationTimeInDays - model.PreparationTimeInDays)
                                            : _calendarService.ExtendPreparationDays(rentalId, model.PreparationTimeInDays - rentalItem.PreparationTimeInDays);
                }
                    
                return new ResourceIdViewModel(){
                    Id = _rentalService.UpdateRental(rentalId, model.Units, model.PreparationTimeInDays)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
