
using Entities.Dto;
using Microsoft.AspNetCore.Identity;

namespace Contracts;

public interface IAuthenticationService
{
   //Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
   Task<bool>  ValidateUser(UserForAuthenticationDto userForAuth);

   Task<string> CreateToken();
}