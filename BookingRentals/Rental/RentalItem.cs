namespace BookingRentals.Rental
{
    public abstract class RentalItem
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}
