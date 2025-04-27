using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int? WrokerId { get; set; }

    public string? TaskName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? TaskStatus { get; set; }

    public byte[]? BeforePhoto { get; set; }

    public byte[]? AfterPhoto { get; set; }

    public string TasksDetails { get; set; } = null!;

    public virtual User? Wroker { get; set; }
}
