namespace Master_Piece.Models
{
    public class AssignedTaskViewModel
    {
        public int TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? TaskStatus { get; set; }
        public string TasksDetails { get; set; } = null!;
        public byte[]? BeforePhoto { get; set; }
        public byte[]? AfterPhoto { get; set; }
    }
}
