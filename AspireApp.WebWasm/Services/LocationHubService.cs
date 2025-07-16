using AspireApp.Shared.Messaging.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace AspireApp.WebWasm.Services;

public class LocationHubService : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    public event Action<LocationChangedNotificationModel>? OnLocationChanged;

    public async Task StartAsync()
    {
        if (_hubConnection != null)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5275/hubs/locations") // Adjust port if needed
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<LocationChangedNotificationModel>("LocationChanged", message =>
        {
            Console.WriteLine($"📢 Received location update: {message.Name}");
            OnLocationChanged?.Invoke(message);
        });

        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}