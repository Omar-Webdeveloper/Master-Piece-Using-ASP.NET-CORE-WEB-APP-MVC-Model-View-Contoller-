namespace Master_Piece.Models
{
    public class ManageEmployeesPageViewModel
    {
        public List<EmployeeWithServiceViewModel> EmployeesWithServices { get; set; }
        public List<MainService> Services { get; set; }
        public List<LocationArea> Locations { get; set; }
    }

}
