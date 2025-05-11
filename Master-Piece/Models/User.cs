using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

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

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingWorkers { get; set; } = new List<Booking>();

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public virtual LocationArea? Location { get; set; }

    public virtual LocationArea? LocationArea { get; set; }

    public virtual ICollection<ServiceWorkersJunctionTable> ServiceWorkersJunctionTables { get; set; } = new List<ServiceWorkersJunctionTable>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
