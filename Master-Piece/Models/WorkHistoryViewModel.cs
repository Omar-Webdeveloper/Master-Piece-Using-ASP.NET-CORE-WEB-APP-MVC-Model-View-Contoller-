namespace Master_Piece.Models
{
    public class WorkHistoryViewModel
    {
        public List<AssignedBookingViewModel> CompletedBookings { get; set; } = new();
        public List<AssignedTaskViewModel> CompletedTasks { get; set; } = new();
    }
}
