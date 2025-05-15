namespace Master_Piece.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TaskStatus { get; set; }
        public string TaskDetails { get; set; }
        public byte[] BeforePhoto { get; set; }
        public byte[] AfterPhoto { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
