using System;
using System.Collections.Generic;
using BookingRentals.Rental;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/vacationrental/rentals")]
    [ApiController]
    public class VacationRentalRentalsController : ControllerBase
    {
        protected readonly IRental _rentalService;
        public VacationRentalRentalsController(IRental rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public CleanedRentalViewModel Get(int rentalId)
        {
            try
            {
                var rentalItem = _rentalService.GetRentalById(rentalId);
                return new CleanedRentalViewModel(){
                    Id = rentalItem.Id,
                    Units = rentalItem.Units,
                    PreparationTimeInDays = rentalItem.PreparationTimeInDays
                };
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        [HttpPost]
        public ResourceIdViewModel Post(CleanedRentalBindingModel model)
        {
            try
            {
                return new ResourceIdViewModel(){
                    Id = _rentalService.AddRental(model.Units, model.PreparationTimeInDays)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
