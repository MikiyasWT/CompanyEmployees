

using System.Net;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
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
    public AuthenticationController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<User> userManager, EmailSenderService emailService)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
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

        //await _emailService.SendEmailAsync(emailDto.To, emailDto.Subject, emailDto.Body);
        await _emailService.SendEmailAsync(userForRegistration.Email, "verify your email", "please verify your godamn email");
        return StatusCode(201);
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
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto resetPassword)
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