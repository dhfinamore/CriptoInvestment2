using System.Security.Claims;
using CryptoInvestment.Application.Authentication.Commands.RegisterCommand;
using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;
using CryptoInvestment.Application.Authentication.Queries.GetCustomerByEmailQuery;
using CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;
using CryptoInvestment.Application.Authentication.Queries.LoginQuery;
using CryptoInvestment.Application.Authentication.Queries.VerifySecurityQuestionsQuery;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.Services.ConfigurationModels;
using CryptoInvestment.ViewModels.Authentication;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CryptoInvestment.Controllers;

public class AuthenticationController : Controller
{
    private readonly ISender _mediator;
    private readonly ICustomerAuthenticationService _customerAuthenticationService;
    private readonly IEmailService _emailService;
    private readonly IEncryptionService _encryptionService;
    private readonly AppSettings _appSettings;

    public AuthenticationController(
        ISender mediator, 
        ICustomerAuthenticationService customerAuthenticationService, 
        IEmailService emailService, 
        IEncryptionService encryptionService,
        IOptions<AppSettings> appSettings)
    {
        _mediator = mediator;
        _customerAuthenticationService = customerAuthenticationService;
        _emailService = emailService;
        _encryptionService = encryptionService;
        _appSettings = appSettings.Value;
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
                HttpContext.Session.SetString("UserEmail", customer.Email);
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
                    var customerQuery = new GetCustomerByEmailQuery(loginViewModel.Email);
                    var getCustomerResult = await _mediator.Send(customerQuery);
                    var customer = getCustomerResult.Match<Customer>(
                        customer => customer,
                        _ => null!
                    );
                    
                    var body = await CreateVerificationEmail(loginViewModel.Email);
                    var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(loginViewModel.Email, "Verifica tu correo", body);
                    return await Task.FromResult<IActionResult>(RedirectToAction(
                        "ConfirmEmail", "Authentication", 
                        new { 
                            name = _encryptionService.EncryptEmail(customer.Nombre!),
                            lastName = _encryptionService.EncryptEmail(customer.ApellidoPaterno!),
                            email = _encryptionService.EncryptEmail(customer.Email)
                        }));
                }
                else if (errors.Any(e => e.Code == "Authentication.SecurityQuestionsNotConfigured"))
                {
                    var customerQuery = new GetCustomerByEmailQuery(loginViewModel.Email);
                    var getCustomerResult = await _mediator.Send(customerQuery);
                    var customer = getCustomerResult.Match<Customer>(
                        customer => customer,
                        _ => null!
                    );
                    
                    return await Task.FromResult<IActionResult>(RedirectToAction(
                        "SecurityQuestions", 
                        "Authentication", 
                        new
                        {
                            token = _encryptionService.EncryptId(customer.IdCustomer.ToString())
                        }));
                }
                else if (errors.Any(e => e.Type == ErrorType.NotFound))
                {
                    ModelState.AddModelError("Email", "El correo ingresado no está registrado.");
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
                else if (errors.Any(e => e.Type == ErrorType.Unauthorized))
                {
                    ModelState.AddModelError("Password", "Contraseña incorrecta.");
                    TempData["Email"] = loginViewModel.Email;
                    TempData["ForgotPasswordMessage"] = "¿Olvidó su contraseña?";
                    return await Task.FromResult<IActionResult>(View(loginViewModel));
                }
                else if (errors.Any(e => e.Code == "Authentication.AccountLocked"))
                {
                    var query2 = new GetCustomerByEmailQuery(loginViewModel.Email);
                    var getCustomerResult = await _mediator.Send(query2);
                    
                    var customer = getCustomerResult.Match<Customer>(
                        customer => customer,
                        _ => null!
                    );

                    if (customer.LockedUp.HasValue)
                    {
                        var minutesLeft = (int) (customer.LockedUp.Value.AddMinutes(15) - DateTime.Now).TotalMinutes + 1;

                        if ((int) (customer.LockedUp.Value.AddMinutes(15) - DateTime.Now).TotalMinutes > 0)
                        {
                            ModelState.AddModelError("Password", $"Su cuenta ha sido bloqueada por demasiados intentos fallidos. Inténtelo de nuevo en {minutesLeft} minutos.");
                        }
                    }

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
    public IActionResult Register(string? token)
    {
        var model = new RegisterViewModel();
        
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                string decryptedValue = _encryptionService.DecryptId(token);
                
                if (int.TryParse(decryptedValue, out int referralId))
                {
                    model.ParentId = referralId;
                }
            }
            catch (Exception ex)
            {
                model.ParentId = null;
            }
        }
        
        return View(model);
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
            registerViewModel.ParentId,
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
                    _ => RedirectToAction(
                        "ConfirmEmail", 
                        "Authentication", 
                        new
                        {
                            name = _encryptionService.EncryptEmail(customer.Nombre!), 
                            lastName = _encryptionService.EncryptEmail(customer.ApellidoPaterno!),
                            email = _encryptionService.EncryptEmail(customer.Email)
                        }),
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
    
    public IActionResult ConfirmEmail(string name, string lastName, string email)
    {
        ViewBag.Name = _encryptionService.DecryptEmail(name);
        ViewBag.LastName = _encryptionService.DecryptEmail(lastName);
        ViewBag.Email = _encryptionService.DecryptEmail(email);
        ViewBag.MailSoporte = _appSettings.MailSoporte;
        ViewBag.TelSoporte = _appSettings.TelSoporte; 
        
        return View();
    }

    public async Task<IActionResult> ResendEmail(string name, string lastName, string email)
    {
        TempData["Resend"] = "true";
        var body = await CreateVerificationEmail(_encryptionService.DecryptEmail(email));
        var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(_encryptionService.DecryptEmail(email), "Verifica tu correo", body);

        ViewBag.Name = _encryptionService.DecryptEmail(name);
        ViewBag.LastName = _encryptionService.DecryptEmail(lastName);
        ViewBag.Email = _encryptionService.DecryptEmail(email);
                
        return View("ConfirmEmail");
    }
    #endregion
    
    # region SetPasswordRegion
    public async Task<IActionResult> SetPassword(string token, string date, bool isReset)
    {
        var sentDate = _encryptionService.DecryptDate(date);
        
        if (sentDate == null || sentDate.Value.AddMinutes(30) < DateTime.UtcNow)
        {
            return RedirectToAction("Login", "Authentication");
        }

        string decryptedEmail = _encryptionService.DecryptEmail(token);

        var query = new GetCustomerByEmailQuery(decryptedEmail);
        var getCustomerResult = await _mediator.Send(query);

        var customer = getCustomerResult.Match(
            c => c,
            e => null!);

        if (!isReset)
        {
            if (customer is not null)
            {
                if (customer.EmailValidated == true)
                {
                    TempData["InfoMessage"] = "Su correo ya fue validado. Por favor, inicie sesión.";
                }
            }
        }
        
        var model = new SetPasswordViewModel
        {
            Email = decryptedEmail
        };

        if (isReset)
        {
            TempData["IsReset"] = "true";
        }

        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel setPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return View(setPasswordViewModel);
        
        var command = new SetPasswordCommand(
            setPasswordViewModel.Email, 
            setPasswordViewModel.Password);
        
        var setPasswordResult = await _mediator.Send(command);
        
        return setPasswordResult.Match<IActionResult>(
            customer => RedirectToAction(
                "SecurityQuestions",
                "Authentication",
                new { token = _encryptionService.EncryptId(customer.IdCustomer.ToString()) }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View(setPasswordViewModel);
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(SetPasswordViewModel setPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Authentication/SetPassword.cshtml", setPasswordViewModel);
        
        var command = new SetPasswordCommand(
            setPasswordViewModel.Email, 
            setPasswordViewModel.Password);
        
        var setPasswordResult = await _mediator.Send(command);
        
        return setPasswordResult.Match<IActionResult>(
            customer => RedirectToAction("Login", "Authentication"),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Authentication/SetPassword.cshtml", setPasswordViewModel);
            }
        );
    }

    public async Task<IActionResult> VerifySecurityQuestions(string token)
    {
        string email = _encryptionService.DecryptEmail(token);
        var query = new GetCustomerByEmailQuery(email);
        var getCustomerResult = await _mediator.Send(query);

        var customer = getCustomerResult.Match(
            c => c,
            e => null!);

        if (customer is null)
            return RedirectToAction("Login", "Authentication");
        
        var query2 = new ListSecurityQuestionsQuery();
        var listSecurityQuestionsResult = await _mediator.Send(query2);
        
        var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
            questions => questions,
            _ => null!
        );
        
        var query3 = new GetCustomerSecurityQuestionsQuery(customer.IdCustomer);
        var getCustomerSecurityQuestionsResult = await _mediator.Send(query3);
        
        var customerQuestions = getCustomerSecurityQuestionsResult.Match<List<CustomerQuestion>>(
            customerQuestions => customerQuestions,
            _ => null!
        );

        var question1 = questions.FirstOrDefault(q => q.IdSecurityQuestion == customerQuestions[0].IdQuestion);
        var question2 = questions.FirstOrDefault(q => q.IdSecurityQuestion == customerQuestions[1].IdQuestion);
        var question3 = questions.FirstOrDefault(q => q.IdSecurityQuestion == customerQuestions[2].IdQuestion);

        if (question1 is null || question2 is null || question3 is null)
            RedirectToAction("Login", "Authentication");
        
        var model = new VerifySecurityQuestionsViewModel()
        {
            Email = email,
            FirstQuestion = question1!.Question,
            SecondQuestion = question2!.Question,
            ThirdQuestion = question3!.Question,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> VerifySecurityQuestions(VerifySecurityQuestionsViewModel verifySecurityQuestionsViewModel)
    {
        if (!ModelState.IsValid)
            return View(verifySecurityQuestionsViewModel);

        var query = new VerifySecurityQuestionsQuery(
            verifySecurityQuestionsViewModel.Email,
            verifySecurityQuestionsViewModel.FirstAnswer!,
            verifySecurityQuestionsViewModel.SecondAnswer!,
            verifySecurityQuestionsViewModel.ThirdAnswer!);

        var verifySecurityQuestionResult = await _mediator.Send(query);

        return verifySecurityQuestionResult.Match<IActionResult>(
            valid => RedirectToAction(
                "SetPassword", 
                "Authentication", 
                new
                {
                    token = _encryptionService.EncryptEmail(verifySecurityQuestionsViewModel.Email),
                    date = _encryptionService.EncryptDate(DateTime.UtcNow),
                    isReset = true
                }),
            errors =>
            {
                if (errors.Any(e => e.Type == ErrorType.NotFound))
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else if (errors.Any(e => e.Type == ErrorType.Unauthorized))
                {
                    ModelState.AddModelError("ThirdAnswer", "Respuestas a las preguntas de seguridad inválidas");
                    return View(verifySecurityQuestionsViewModel);
                }
                else
                {
                    ModelState.AddModelError("", "Unexpected Error");
                    return View(verifySecurityQuestionsViewModel);
                }
            });
    }

    # endregion
    
    # region SecurityQuestionsRegion
    [HttpPost]
    public async Task<IActionResult> SecurityQuestions(SecurityQuestionsViewModel securityQuestionsViewModel)
    {
        if (!ModelState.IsValid)
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
            
            var model = new SecurityQuestionsViewModel()
            {
                CustomerId = securityQuestionsViewModel.CustomerId,
                SecurityQuestions = questions
            };
            
            return (View(model));
        }

        List<(int, string)> securityQuestions =
        [
            (securityQuestionsViewModel.FirstQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(securityQuestionsViewModel.FirstQuestionAnswer)),
            (securityQuestionsViewModel.SecondQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(securityQuestionsViewModel.SecondQuestionAnswer)),
            (securityQuestionsViewModel.ThirdQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(securityQuestionsViewModel.ThirdQuestionAnswer))
        ];
        
        var command = new SetSecurityQuestionCommand(securityQuestionsViewModel.CustomerId, securityQuestions);
        var setSecurityQuestionResult = await _mediator.Send(command);
        
        return setSecurityQuestionResult.Match<IActionResult>(
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

                return RedirectToAction("Dashboard", "Crypto");
            },
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View(securityQuestionsViewModel);
            }
        );
    }
    
    public async Task<IActionResult> SecurityQuestions(string token)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Authentication");
                
        string decryptedValue = _encryptionService.DecryptId(token);
        
        var query = new ListSecurityQuestionsQuery();
        var listSecurityQuestionsResult = await _mediator.Send(query);
        
        var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
            questions => questions,
            _ => null!
        );
        
        var model = new SecurityQuestionsViewModel()
        {
            CustomerId = int.Parse(decryptedValue),
            SecurityQuestions = questions
        };
        
        return View(model);
    }
    #endregion

    #region LogoutRegion

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Authentication");
    }
    #endregion
    
    #region HelpersRegion
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
    #endregion
}
