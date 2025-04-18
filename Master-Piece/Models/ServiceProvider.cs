﻿using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class ServiceProvider
{
    public int ProviderId { get; set; }

    public int? UserId { get; set; }

    public string? WorkerName { get; set; }

    public string? ServiceType { get; set; }

    public double? Rating { get; set; }

    public string? Achievements { get; set; }

    public string? Intro { get; set; }

    public string? Photos { get; set; }

    public DateTime? RegisterAt { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? WorkerAddress { get; set; }

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual User? User { get; set; }
}
