using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Achievement
{
    public int AchievementId { get; set; }

    public string AchievementName { get; set; } = null!;

    public byte[]? AchievementPatchImage { get; set; }

    public string? AchievementDescription { get; set; }

    public DateOnly? AchievementDate { get; set; }
}
