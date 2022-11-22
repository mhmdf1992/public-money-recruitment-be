namespace BookingRentals.Rental
{
    public interface IRental
    {
        RentalItem GetRentalById(int id);
        int AddRental(int units, int preparationTimeInDays = 0);
        int UpdateRental(int id, int units, int preparationTimeInDays);
    }
}
