using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NotificationService.Samples;

public interface ISampleAppService : IApplicationService
{
    Task SubscriberAsync();

    Task PublisherAsync();
}
