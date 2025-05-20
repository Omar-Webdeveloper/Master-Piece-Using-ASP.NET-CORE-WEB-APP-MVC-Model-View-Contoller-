namespace Master_Piece.Models
{
    public class AllManagersViewModel
    {
        public List<User> All_Users { get; set; } = new List<User>(); // ✅ Holds all Users
        public List<User> All_Employees { get; set; } = new List<User>(); // ✅ Holds all EMPLOYEES

        public List<User> Managers { get; set; } = new List<User>(); // ✅ Holds all managers
        public User AllManagers { get; set; } = new User(); // ✅ Holds a single manager instance if needed
        public List<LocationArea> Locations { get; set; } = new List<LocationArea>(); // ✅ List of available locations
    }
}
