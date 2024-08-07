using System.Net;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    private readonly IMapper _mapper;

    private readonly UserManager<User> _userManager;

    private readonly EmailSenderService _emailService;

    private readonly IServiceManager _service;

    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IConfiguration _configuration;
    public AuthenticationController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<User> userManager, EmailSenderService emailService, IServiceManager service, IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
        _service = service;
        _configuration = configuration;
        _jwtConfiguration = new JwtConfiguration();
        _configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);



    }


    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _service.AuthenticationService.RegisterUser(userForRegistration);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(userForRegistration.Email);

        if (user == null)
        {
            return BadRequest("User not found after registration.");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
        await _service.EmailSenderService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: {confirmationLink}");
        return StatusCode(201);
    }



    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
    {
        if (!await _service.AuthenticationService.ValidateUser(userForAuthentication))
        {
            return Unauthorized();
        }

        var tokenDTO = await _service.AuthenticationService.CreateToken(populateExp: true);

        return Ok(tokenDTO);
    }


    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto confirmEmailDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
        if (user == null)
        {
            _logger.LogInfo($"No such user with this email: {confirmEmailDto.Email}");
            return NotFound($"No user was found, Invalid Email");
        }

        var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);
        if (!result.Succeeded)
        {
            _logger.LogInfo($"Unable to verify email for user: {confirmEmailDto.Email}");
            return BadRequest($"Email verification failed for user: {confirmEmailDto.Email}");
        }

        return Ok("Email confirmed successfully");
    }



    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPassword)
    {
        var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);

        // var message = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";
        var message = $"Please use this token to reset your password: {token}";
        await _emailService.SendEmailAsync(forgotPassword.Email, "Reset Password", message);

        return Ok("Password reset link has been sent to your email.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(resetPassword.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        return Ok("Password has been reset successfully.");
    }
}