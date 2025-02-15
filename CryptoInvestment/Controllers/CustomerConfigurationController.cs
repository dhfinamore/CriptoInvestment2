using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;
using CryptoInvestment.ViewModels.CustomerConfiguration;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CryptoInvestment.Controllers;

[Authorize]
public class CustomerConfigurationController : Controller
{
    private readonly ISender _mediator;

    public CustomerConfigurationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForResetPassword(ModelState);

        if (!ModelState.IsValid)
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);

        var email = HttpContext.Session.GetString("UserEmail");
        
        if (email is null)
            return RedirectToAction("Login", "Authentication");
        
        var command = new SetPasswordCommand(
            email,
            customerConfigurationViewModel.ResetPassword.Password,
            customerConfigurationViewModel.ResetPassword.CurrentPassword);

        var setPasswordResult = await _mediator.Send(command);
        
        return setPasswordResult.Match<IActionResult>(
            customer => RedirectToAction("CustomerConfiguration", "Crypto"),
            error =>
            {
                if (error.Any(e => e.Type == ErrorType.NotFound))
                    ModelState.AddModelError("", "No existe un usuario con ese email.");
                else if (error.Any(e => e.Type == ErrorType.Forbidden))
                    ModelState.AddModelError("ResetPassword.CurrentPassword", "La contraseña actual no es correcta");
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error inesperado.");
                }

                return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
            });
    }
    
    [HttpPost]
    public async Task<IActionResult> ChangeSecurityQuestions(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForChangeSecurityQuestions(ModelState);
        
        if (!ModelState.IsValid)
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        
        List<(int, string)> securityQuestions =
        [
            (customerConfigurationViewModel.SetSecurityQuestion.FirstQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(customerConfigurationViewModel.SetSecurityQuestion.FirstQuestionAnswer)),
            (customerConfigurationViewModel.SetSecurityQuestion.SecondQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(customerConfigurationViewModel.SetSecurityQuestion.SecondQuestionAnswer)),
            (customerConfigurationViewModel.SetSecurityQuestion.ThirdQuestionId, 
                BCrypt.Net.BCrypt.HashPassword(customerConfigurationViewModel.SetSecurityQuestion.ThirdQuestionAnswer))
        ];
        
        var command = new SetSecurityQuestionCommand(customerConfigurationViewModel.CustomerId, securityQuestions, true);
        var setSecurityQuestionResult = await _mediator.Send(command);
        
        return setSecurityQuestionResult.Match<IActionResult>(
            customer => RedirectToAction("CustomerConfiguration", "Crypto"),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml",customerConfigurationViewModel);
            }
        );
    }
    
    private void RemoveValidationForResetPassword(ModelStateDictionary modelState)
    {
        modelState.Remove("SetSecurityQuestion.FirstQuestionId");
        modelState.Remove("SetSecurityQuestion.FirstQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecondQuestionId");
        modelState.Remove("SetSecurityQuestion.SecondQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionId");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecurityQuestions");
    }
    
    private void RemoveValidationForChangeSecurityQuestions(ModelStateDictionary modelState)
    {
        modelState.Remove("ResetPassword.CurrentPassword");
        modelState.Remove("ResetPassword.Password");
        modelState.Remove("ResetPassword.ConfirmPassword");
    }

}