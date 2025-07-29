namespace AspireApp.MasterDataService.Messages.Events;

public record LocationDeletedEvent(int Id, string Name, DateTime DeletedAt)
{
    public LocationDeletedEvent(int id, string name) 
        : this(id, name, DateTime.UtcNow)
    {
    }
} 