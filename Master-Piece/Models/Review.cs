using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? BookingId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }
}
