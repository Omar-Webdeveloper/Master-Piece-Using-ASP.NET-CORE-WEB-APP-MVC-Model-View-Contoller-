namespace Master_Piece.Models
{
    public class EmployeeWithServiceViewModel
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public byte[]? PersonalImage { get; set; }
        public string? PersonalAddress { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public int? LocationId { get; set; }
        public string? WorkerServiceType { get; set; }
        public double? WorkerRating { get; set; }
        public string? WorkerIntro { get; set; }
        public bool? IsActive { get; set; }
        public string? ServiceName { get; set; }
    }
}
