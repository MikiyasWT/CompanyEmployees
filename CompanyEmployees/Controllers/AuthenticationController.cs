

using System.Net;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    private readonly IMapper _mapper;

    private readonly UserManager<User> _userManager;
    public AuthenticationController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,  UserManager<User> userManager)
    {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody]UserForRegistrationDto userForRegistration)
    {
        // map userForRegistration into User
        var user = _mapper.Map<User>(userForRegistration);

        var result = await _userManager.CreateAsync(user, userForRegistration.Password);
        if(!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        await _userManager.AddToRolesAsync(user, userForRegistration.Roles);

        return StatusCode(201);
    }

}