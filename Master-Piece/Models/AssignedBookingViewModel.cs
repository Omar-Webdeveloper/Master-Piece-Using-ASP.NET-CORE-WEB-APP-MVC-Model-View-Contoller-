namespace Master_Piece.Models
{
    public class AssignedBookingViewModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Personal_Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceName { get; set; }
        public int BookingId { get; set; }
        public DateTime Booking_Start_Date { get; set; }
        public string BookingTittle { get; set; }
        public string BookingMessae { get; set; }
        public string BookingNotes { get; set; }
        public byte[] ImageWhereTheIssueLocated { get; set; }
        public int? WorkerID { get; set; }
        public string? Status { get; set; }
        public byte[]? ImageAfterFixing { get; set; }


    }

}
