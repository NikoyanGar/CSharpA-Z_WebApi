using Microsoft.AspNetCore.SignalR;

namespace SignalRExplain1.Hubs
{
    // The NotificationsHub class inherits from the SignalR Hub class
    // This class is used to handle real-time communication between server and clients
    public sealed class NotificationsHub : Hub
    {
        // This method sends a notification to all connected clients
        // The method is asynchronous and returns a Task
        public async Task SendNotification(string content)
        {
            // Clients.All.SendAsync sends a message to all connected clients
            // "ReceiveNotification" is the name of the client-side method to be invoked
            // content is the message to be sent to the clients
            await Clients.All.SendAsync("ReceiveNotification", content);
        }
    }
}
