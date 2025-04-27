using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class WorkerAchievement
{
    public int WorkerId { get; set; }

    public int AchievementId { get; set; }

    public virtual Achievement Achievement { get; set; } = null!;

    public virtual User Worker { get; set; } = null!;
}
