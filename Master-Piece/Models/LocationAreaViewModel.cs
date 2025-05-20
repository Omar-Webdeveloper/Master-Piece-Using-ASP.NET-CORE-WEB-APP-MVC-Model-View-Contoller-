namespace Master_Piece.Models
{
    public class LocationWithManager
    {
        public string AreasCovered { get; set; }
        public string ManagerName { get; set; }
        public int LocationId { get; set; }
    }
    public class LocationDto
    {
        public int LocationId { get; set; }
        public string AreasCovered { get; set; }
    }

    public class LocationAreaViewModel
    {
        public List<LocationDto> LocationsWithoutManagers { get; set; }
        public List<LocationWithManager> LocationsWithManagers { get; set; }
        public LocationArea OneLocation { get; set; } = new LocationArea();
    }
}
