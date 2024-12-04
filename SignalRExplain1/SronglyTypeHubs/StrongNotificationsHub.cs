using Microsoft.AspNetCore.SignalR;
using SignalRExplain1.SronglyTypeHubs.ClientInterfaces;

namespace SignalRExplain1.SronglyTypeHubs
{
    //You will lose access to the SendAsync method, and only the methods defined in your client interface will be available.
    public sealed class StrongNotificationsHub : Hub<INotificationsClient>
    {
        public async Task SendNotification(string content)
        {
            await Clients.All.ReceiveNotification(content);
        }
    }
}
