using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? ServiceId { get; set; }

    public string? Status { get; set; }

    public DateTime? BookingStartDate { get; set; }

    public DateTime? BookingEndDate { get; set; }

    public string? BookingTittle { get; set; }

    public string? BookingMessae { get; set; }

    public string? BookingNotes { get; set; }

    public byte[]? ImageWhereTheIssueLocated { get; set; }

    public int? WorkerId { get; set; }

    public byte[]? ImageAfterFixing { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual MainService? Service { get; set; }

    public virtual User? User { get; set; }

    public virtual User? Worker { get; set; }
}
