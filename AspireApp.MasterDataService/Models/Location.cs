namespace AspireApp.MasterDataService.Models;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }  // e.g., Site, Room, Area
    public int? ParentId { get; set; } // null if top-level
}