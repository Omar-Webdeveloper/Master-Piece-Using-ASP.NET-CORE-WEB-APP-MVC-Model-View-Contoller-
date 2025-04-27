using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Evaluation
{
    public int EvaluationId { get; set; }

    public int? WrokerId { get; set; }

    public int? EvaluationYear { get; set; }

    public double? Score { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Wroker { get; set; }
}
