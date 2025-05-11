using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class MainService
{
    public int ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public decimal? ServicePrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ServiceWorkersJunctionTable> ServiceWorkersJunctionTables { get; set; } = new List<ServiceWorkersJunctionTable>();
}
