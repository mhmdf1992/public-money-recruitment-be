using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Rental
{
    public class IdentifiedRentalItem : BookingRentals.Rental.RentalItem, Repository.Identified
    {
        public string Key { get => Id.ToString(); }
    }
}
