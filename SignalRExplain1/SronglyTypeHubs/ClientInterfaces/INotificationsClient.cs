namespace SignalRExplain1.SronglyTypeHubs.ClientInterfaces
{
    //The arguments don't have to be primitive types and can also be objects. SignalR will take care of serialization on the client side.
    public interface INotificationsClient
    {
        Task ReceiveNotification(string content);
    }
}
