using System.ComponentModel.DataAnnotations;

namespace Master_Piece.Models
{
    public class HomePageViewModel
    {
        // List of services to show on the home page
        public List<MainServiceViewModel> MainServices { get; set; }

        // List of reviews to show on the home page
        public List<ReviewViewModel> RandomReviews { get; set; }

        // Contact Us form properties
        public ContactUs ContactUsForm { get; set; }
    }
    public class MainServiceViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }

    public class ReviewViewModel
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public byte[] ImageWhereTheIssueLocated { get; set; }
        public byte[] ImageAfterFixing { get; set; }
        public string UserFirstName { get; set; }
    }
    public class ContactUs
    {
        [Required(ErrorMessage = "First Name is required.")]

        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]

        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(2000, ErrorMessage = "Message cannot be longer than 2000 characters.")]
        public string ContactUsMessage { get; set; }
    }

}
