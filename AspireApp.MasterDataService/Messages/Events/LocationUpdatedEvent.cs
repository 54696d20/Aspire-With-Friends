namespace AspireApp.MasterDataService.Messages.Events;

public record LocationUpdatedEvent(int Id, string Name, string Type, int? ParentId, DateTime UpdatedAt)
{
    public LocationUpdatedEvent(int id, string name, string type, int? parentId) 
        : this(id, name, type, parentId, DateTime.UtcNow)
    {
    }
} 