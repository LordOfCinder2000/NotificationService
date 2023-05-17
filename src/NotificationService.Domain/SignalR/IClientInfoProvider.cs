namespace NotificationService.SignalR;

public interface IClientInfoProvider
{
    string BrowserInfo { get; }

    string ClientIpAddress { get; }

    string ComputerName { get; }
}