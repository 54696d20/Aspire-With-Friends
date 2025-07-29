using AspireApp.MasterDataService.Messages.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace AspireApp.MasterDataService.Messages.Events;

public class LocationEventHandlers
{
    private readonly ILogger<LocationEventHandlers> _logger;

    public LocationEventHandlers(ILogger<LocationEventHandlers> logger)
    {
        _logger = logger;
    }

    public void Handle(LocationCreatedEvent @event)
    {
        _logger.LogInformation("Location created: {LocationId} - {LocationName} ({LocationType})", 
            @event.Id, @event.Name, @event.Type);
        
        // Here you could:
        // - Update search indexes
        // - Send notifications to other services
        // - Update caches
        // - Trigger workflows
    }

    public void Handle(LocationUpdatedEvent @event)
    {
        _logger.LogInformation("Location updated: {LocationId} - {LocationName} ({LocationType})", 
            @event.Id, @event.Name, @event.Type);
        
        // Here you could:
        // - Invalidate caches
        // - Update search indexes
        // - Notify dependent services
    }

    public void Handle(LocationDeletedEvent @event)
    {
        _logger.LogInformation("Location deleted: {LocationId} - {LocationName}", 
            @event.Id, @event.Name);
        
        // Here you could:
        // - Clean up related data
        // - Remove from search indexes
        // - Notify dependent services
    }
} 