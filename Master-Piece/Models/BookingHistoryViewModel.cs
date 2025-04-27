namespace Master_Piece.Models
{
    public class BookingHistoryViewModel
    {
        public int BookingId { get; set; }
        public string BookingTittle { get; set; }
        public string BookingMessae { get; set; }
        public string Status { get; set; }
        public DateTime? BookingStartDate { get; set; }
        public string ServiceName { get; set; }
        public string EmployeeFullName { get; set; }
        public byte[] IssueImage { get; set; }
        public decimal? ServicePrice { get; set; }

    }
}
