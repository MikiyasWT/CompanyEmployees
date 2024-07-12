using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmailSenderService> _emailSenderService;


    public ServiceManager(IConfiguration configuration)
    {
        _emailSenderService = new Lazy<IEmailSenderService>(() => new EmailSenderService(configuration));

    }


    public IEmailSenderService EmailSenderService => _emailSenderService.Value;
}

