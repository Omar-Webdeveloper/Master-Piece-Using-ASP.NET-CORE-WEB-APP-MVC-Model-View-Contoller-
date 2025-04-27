namespace Master_Piece.Models
{
    public class ShowWorkersAndBookingViewModel
    {
        public List<User> Employees { get; set; }
        public Booking NewBooking { get; set; } = new Booking();
        public int ServiceId { get; set; }
    }
}
