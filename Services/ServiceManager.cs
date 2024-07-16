using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmailSenderService> _emailSenderService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
    {
        _emailSenderService = new Lazy<IEmailSenderService>(() => new EmailSenderService(configuration, logger));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, logger, mapper, configuration));
    }


    public IEmailSenderService EmailSenderService => _emailSenderService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}

