using System.Security.Claims;
using CryptoInvestment.Application.Authentication.Commands.RegisterCommand;
using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Queries.LoginQuery;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.ViewModels.Authentication;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CryptoInvestment.Controllers;

public class AuthenticationController : Controller
{
    private readonly ISender _mediator;
    private readonly ICustomerAuthenticationService _customerAuthenticationService;
    private readonly IEmailService _emailService;
    private readonly IEncryptionService _encryptionService;

    public AuthenticationController(
        ISender mediator, 
        ICustomerAuthenticationService customerAuthenticationService, 
        IEmailService emailService, 
        IEncryptionService encryptionService)
    {
        _mediator = mediator;
        _customerAuthenticationService = customerAuthenticationService;
        _emailService = emailService;
        _encryptionService = encryptionService;
    }

    # region LoginRegion
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
            return View(loginViewModel);
        
        var query = new LoginQuery(loginViewModel.Email, loginViewModel.Password);
        var loginResult = await _mediator.Send(query);

        return await loginResult.MatchAsync<IActionResult>(
            customer =>
            {
                HttpContext.Session.SetString("UserId", customer.IdCustomer.ToString());
                var identity = _customerAuthenticationService.CreateClaimsIdentityAsync(customer).Result;
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties).Wait();

                return Task.FromResult<IActionResult>(RedirectToAction("Dashboard", "Crypto"));
            }, async errors =>
            {
                if (errors.Any(e => e.Code == "Authentication.EmailNotVerified"))
                {
                    var body = await CreateVerificationEmail(loginViewModel.Email);
                    var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(loginViewModel.Email, "Verifica tu correo", body);
                    return await Task.FromResult<IActionResult>(RedirectToAction("ConfirmEmail", "Authentication", new { email = loginViewModel.Email }));
                }
                else if (errors.Any(e => e.Code == "Authentication.SecurityQuestionsNotConfigured"))
                {
                    // TODO: Implementar lógica para encriptar el id del cliente y redirigir a la vista de preguntas de seguridad
                    return await Task.FromResult<IActionResult>(RedirectToAction("SecurityQuestions", "Authentication"));
                }
                else if (errors.Any(e => e.Type == ErrorType.NotFound))
                {
                    ModelState.AddModelError("Email", "El correo ingresado no está registrado.");
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
                else if (errors.Any(e => e.Type == ErrorType.Unauthorized))
                {
                    ModelState.AddModelError("Password", "Contraseña incorrecta.");
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
                else if (errors.Any(e => e.Code == "Authentication.AccountLocked"))
                {
                    ModelState.AddModelError("Password", "Su cuenta ha sido bloqueada por demasiados intentos fallidos. Inténtelo de nuevo en 2 minutos.");
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
                else
                {
                    ModelState.AddModelError("", errors.First().Description);
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
            }
        );
    }
    # endregion

    #region RegisterRegion
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
            return View(registerViewModel);

        if (!registerViewModel.TermsAndConditions)
        {
            ModelState.AddModelError("TermsAndConditions", "Debe aceptar los términos y condiciones.");
            return View(registerViewModel);
        }
        
        var command = new RegisterCommand(
            registerViewModel.Email, 
            registerViewModel.Name, 
            registerViewModel.FirstFamilyName, 
            registerViewModel.SecondFamilyName, 
            registerViewModel.Phone, 
            registerViewModel.TermsAndConditions, 
            registerViewModel.AcceptPromotions);
        
        var registerResult = await _mediator.Send(command);
        
        return await registerResult.MatchAsync<IActionResult>(async customer =>
            {
                var body = await CreateVerificationEmail(customer.Email);
                var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(customer.Email, "Verifica tu correo", body);

                return sendVerificationEmailResult.Match<IActionResult>(
                    _ => RedirectToAction("ConfirmEmail", "Authentication", new { email = customer.Email }),
                    errors =>
                    {
                        ModelState.AddModelError("", errors.First().Description);
                        return View(registerViewModel);
                    }
                );
            },
            errors =>
            {
                if (errors.Any(e => e.Code == "Register.EmailAlreadyRegistered"))
                {
                    ModelState.AddModelError("Email", $"{errors.First(e => e.Code == "Register.EmailAlreadyRegistered").Description}");
                }
                else if (errors.Any(e => e.Code == "Register.PhoneAlreadyRegistered"))
                {
                    ModelState.AddModelError("Phone", $"{errors.First(e => e.Code == "Register.PhoneAlreadyRegistered").Description}");
                }
                else
                {
                    ModelState.AddModelError("", errors.First().Description);
                }

                return Task.FromResult<IActionResult>(View(registerViewModel));
            }
        );
    }
    
    public IActionResult ConfirmEmail(string email)
    {
        ViewBag.Email = email;
        return View();
    }
    #endregion
    
    # region SetPasswordRegion
    [HttpPost]
    public IActionResult SetPassword(SetPasswordViewModel setPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return View(setPasswordViewModel);
        
        var command = new SetPasswordCommand(
            setPasswordViewModel.Email, 
            setPasswordViewModel.Password);
        
        var setPasswordResult = _mediator.Send(command).Result;
        
        return setPasswordResult.Match<IActionResult>(
            customer => RedirectToAction(
                "SecurityQuestions", 
                "Authentication", 
                new
                {
                    token = _encryptionService.EncryptId(customer.IdCustomer.ToString())
                }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View(setPasswordViewModel);
            }
        );
    }
    
    public IActionResult SetPassword(string token, string date)
    {
        var sentDate = _encryptionService.DecryptDate(date);
        
        if (sentDate == null || sentDate.Value.AddMinutes(30) < DateTime.UtcNow)
        {
            return RedirectToAction("Login", "Authentication");
        }

        string decryptedEmail = _encryptionService.DecryptEmail(token);
        
        var model = new SetPasswordViewModel
        {
            Email = decryptedEmail
        };

        return View(model);
    }
    # endregion
    
    
    public IActionResult SecurityQuestions(string token)
    {
        Console.WriteLine(token);
        return View();
    }

    private async Task<string> CreateVerificationEmail(string email)
    {
        string token = _encryptionService.EncryptEmail(email);
        string encryptedDate = _encryptionService.EncryptDate(DateTime.UtcNow);
        
        string verificationLink = Url.Action(
            "SetPassword", 
            "Authentication", 
            new { token, date = encryptedDate }, 
            Request.Scheme)!;

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "VerifyEmailTemplate.html");

        string bodyTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

        string body = bodyTemplate
            .Replace("{Name}", email)
            .Replace("{VerificationLink}", verificationLink);

        return body;
    }
}
