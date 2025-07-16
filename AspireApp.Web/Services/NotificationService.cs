using Microsoft.AspNetCore.SignalR.Client;

namespace AspireApp.Web.Services
{
    public class NotificationService
    {
        private HubConnection? _hubConnection;

        public event Func<string, Task>? OnNotificationReceived;
        
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5196/notifications")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("Notify", async (message) =>
            {
                _logger.LogInformation($"📥 Received notification: {message}");
                if (OnNotificationReceived != null)
                    await OnNotificationReceived.Invoke(message);
            });

            await _hubConnection.StartAsync();
            _logger.LogInformation("🔌 SignalR connection started.");
        }
    }
}