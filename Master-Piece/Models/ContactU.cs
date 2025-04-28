using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Master_Piece.Models;

public partial class ContactU
{
    public int ContactId { get; set; }
    [Required(ErrorMessage = "First Name is required.")]

    public string FirstName { get; set; } = null!;
    [Required(ErrorMessage = "Last Name is required.")]

    public string LastName { get; set; } = null!;
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]

    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Message is required.")]
    [StringLength(2000, ErrorMessage = "Message cannot be longer than 2000 characters.")]
    public string ContactUsMessage { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
