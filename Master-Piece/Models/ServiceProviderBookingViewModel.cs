namespace Master_Piece.Models
{
    public class ServiceProviderBookingViewModel
    {
        public  IEnumerable<ServiceProvider>? Providers { get; set; }
        public Booking? Booking { get; set; }
    }
}
