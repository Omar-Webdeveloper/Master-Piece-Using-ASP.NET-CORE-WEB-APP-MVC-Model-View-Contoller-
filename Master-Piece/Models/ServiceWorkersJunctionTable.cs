using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class ServiceWorkersJunctionTable
{
    public int? WrokerId { get; set; }

    public int? ServiceId { get; set; }

    public virtual MainService? Service { get; set; }

    public virtual User? Wroker { get; set; }
}
