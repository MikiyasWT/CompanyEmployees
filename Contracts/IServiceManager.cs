

namespace Contracts;
	public interface IServiceManager
	{
		IEmailSenderService EmailSenderService { get; }
		IAuthenticationService AuthenticationService {get;}
	}