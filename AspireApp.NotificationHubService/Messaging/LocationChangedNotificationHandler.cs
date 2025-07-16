using AspireApp.NotificationHubService.Hubs;
using AspireApp.Shared.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace AspireApp.NotificationHubService.Handlers;

public class LocationChangedNotificationHandler
{
    private readonly IHubContext<LocationHub> _hubContext;
    private readonly ILogger<LocationChangedNotificationHandler> _logger;

    public LocationChangedNotificationHandler(
        IHubContext<LocationHub> hubContext,
        ILogger<LocationChangedNotificationHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    
    [WolverineHandler]
    public async Task Handle(LocationChangedNotificationModel message)
    {
        _logger.LogInformation("✅ Handling LocationChangedNotification in Hub Service for {Location}", message.Name);

        await _hubContext.Clients.All.SendAsync("LocationChanged", message);
    }
}