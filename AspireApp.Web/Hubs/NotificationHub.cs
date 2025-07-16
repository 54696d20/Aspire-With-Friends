using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AspireApp.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"✅ SignalR client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
    }
}