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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactUsMessage { get; set; }
    }

}
