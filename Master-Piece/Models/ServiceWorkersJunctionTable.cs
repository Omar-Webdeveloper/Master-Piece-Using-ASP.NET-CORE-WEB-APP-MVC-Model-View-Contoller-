using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class ServiceWorkersJunctionTable
{
    public int? ProviderId { get; set; }

    public int? ServiceId { get; set; }

    public virtual ServiceProvider? Provider { get; set; }

    public virtual Service? Service { get; set; }
}
