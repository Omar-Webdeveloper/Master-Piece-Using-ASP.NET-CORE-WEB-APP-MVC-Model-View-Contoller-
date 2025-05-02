using System.ComponentModel.DataAnnotations;

namespace Master_Piece.Models
{
    public class Register_New_UserViewModel
    {
        [Required(ErrorMessage = "Your First Name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Your Last Name is required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 16 characters")]

        public string? PasswordHash { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }


        public byte[]? PersonalImage { get; set; }
        [Required(ErrorMessage = "Personal Address is required")]
        public string? PersonalAddress { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^00962\d{9}$", ErrorMessage = "Phone number must start with 00962 followed by 9 digits")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }
    }
}
