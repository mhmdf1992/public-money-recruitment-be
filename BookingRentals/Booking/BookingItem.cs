namespace BookingRentals.Booking
{
    public abstract class BookingItem
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public System.DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}
