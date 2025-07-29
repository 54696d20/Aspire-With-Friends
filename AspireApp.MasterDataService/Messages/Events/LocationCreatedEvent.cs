namespace AspireApp.MasterDataService.Messages.Events;

public record LocationCreatedEvent(int Id, string Name, string Type, int? ParentId, DateTime CreatedAt)
{
    public LocationCreatedEvent(int id, string name, string type, int? parentId) 
        : this(id, name, type, parentId, DateTime.UtcNow)
    {
    }
} 