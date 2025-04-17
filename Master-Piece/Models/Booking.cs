using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? ServiceId { get; set; }

    public string? Status { get; set; }

    public DateTime? BookingDate { get; set; }

    public string? BookingTittle { get; set; }

    public string? BookingMessae { get; set; }

    public string? BookingNotes { get; set; }

    public byte[]? ImageWhereTheIssueLocated { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Service? Service { get; set; }

    public virtual User? User { get; set; }
}
