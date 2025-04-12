using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public decimal? StartingPrices { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
