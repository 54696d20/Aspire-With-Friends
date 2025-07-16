using AspireApp.MasterDataService.Messages;
using AspireApp.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AspireApp.Web.Handlers;

public class LocationNotificationHandler
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<LocationNotificationHandler> _logger;

    public LocationNotificationHandler(
        IHubContext<NotificationHub> hubContext, 
        ILogger<LocationNotificationHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(LocationChangedNotification message)
    {
        _logger.LogInformation("📨 Web received notification: {Message}", message.Message);
        await _hubContext.Clients.All.SendAsync("Notify", message.Message);
    }
}