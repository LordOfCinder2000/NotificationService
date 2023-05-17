namespace NotificationService.SignalR;
public class ClientStoreCacheKey
{
    public string ConnectionId { get; set; }

    //Builds the cache key
    public override string ToString()
    {
        return $"OnlineClients:Clients";
    }
}
