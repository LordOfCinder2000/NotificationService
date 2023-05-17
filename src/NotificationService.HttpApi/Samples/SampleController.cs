using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Notifications;
using Volo.Abp;

namespace NotificationService.Samples;

[Area(NotificationServiceRemoteServiceConsts.ModuleName)]
[RemoteService(Name = NotificationServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/NotificationService")]
public class SampleController : NotificationServiceController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public SampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpPost]
    [Route("Subscriber")]
    public async Task SubscriberAsync()
    {
        await _sampleAppService.SubscriberAsync();
    }

    [HttpPost]
    [Route("Publisher")]
    public async Task PublisherAsync()
    {
        await _sampleAppService.PublisherAsync();
    }
}
