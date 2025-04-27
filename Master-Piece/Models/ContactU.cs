using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class ContactU
{
    public int ContactId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactUsMessage { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
