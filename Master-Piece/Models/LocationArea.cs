using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class LocationArea
{
    public int LocationId { get; set; }

    public string? AreasCovered { get; set; }

    public int? ManagerId { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
