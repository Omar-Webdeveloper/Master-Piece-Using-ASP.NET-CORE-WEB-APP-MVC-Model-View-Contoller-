using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int? ProviderId { get; set; }

    public string? TaskName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? TaskStatus { get; set; }

    public string? BeforePhoto { get; set; }

    public string? AfterPhoto { get; set; }

    public string TasksDetails { get; set; } = null!;

    public virtual ServiceProvider? Provider { get; set; }
}
