using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Command.AssignPercentageCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Command.CreateBeneficiaryCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Command.DeleteBeneficiaryCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.SecurityQuestions;
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
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
            
            customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
            
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

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
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
                
            customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
            
            var query3 = new GetCustomerBeneficiariesQuery(customerConfigurationViewModel.CustomerId);
            var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
            var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
                customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
                _ => null!
            );

            customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
            
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }
        
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
            customer => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "security-questions" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml",customerConfigurationViewModel);
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBeneficiary(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForCreateBeneficiary(ModelState);

        if (!ModelState.IsValid)
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
            
            customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
            
            var query3 = new GetCustomerBeneficiariesQuery(customerConfigurationViewModel.CustomerId);
            var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
            var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
                customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
                _ => null!
            );

            customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
            
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

        var command = new CreateBeneficiaryCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerBeneficiary.Name,
            customerConfigurationViewModel.CustomerBeneficiary.ApePaternal,
            customerConfigurationViewModel.CustomerBeneficiary.ApeMaternal,
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber,
            customerConfigurationViewModel.CustomerBeneficiary.Relationship
        );
        
        var createCustomerResult = await _mediator.Send(command);
        
        return createCustomerResult.Match<IActionResult>(
            customer => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "ben" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml",customerConfigurationViewModel);
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateBeneficiary(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForCreateBeneficiary(ModelState);

        if (!ModelState.IsValid)
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
            
            customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
            
            var query3 = new GetCustomerBeneficiariesQuery(customerConfigurationViewModel.CustomerId);
            var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
            var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
                customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
                _ => null!
            );

            customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
            
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

        var command = new CreateBeneficiaryCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerBeneficiary.Name,
            customerConfigurationViewModel.CustomerBeneficiary.ApePaternal,
            customerConfigurationViewModel.CustomerBeneficiary.ApeMaternal,
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber,
            customerConfigurationViewModel.CustomerBeneficiary.Relationship,
            customerConfigurationViewModel.CustomerBeneficiary.Id
        );
        
        var createCustomerResult = await _mediator.Send(command);
        
        return createCustomerResult.Match<IActionResult>(
            customer => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "ben" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml",customerConfigurationViewModel);
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> AssignBeneficiaryPercent(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForAssignBeneficiaryPercent(ModelState);

        if (!ModelState.IsValid)
        {
            var query = new ListSecurityQuestionsQuery();
            var listSecurityQuestionsResult = await _mediator.Send(query);
        
            var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
                questions => questions,
                _ => null!
            );
            
            customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
            
            var query3 = new GetCustomerBeneficiariesQuery(customerConfigurationViewModel.CustomerId);
            var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
            var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
                customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
                _ => null!
            );

            customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
            
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

        var command = new AssignPercentageCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries
        );
        
        var assignBeneficiaryPercentResult = await _mediator.Send(command);
        
        return assignBeneficiaryPercentResult.Match<IActionResult>(
            customer => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "ben" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml",customerConfigurationViewModel);
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteBeneficiary(int customerId, int beneficiaryId)
    {
        var command = new DeleteBeneficiaryCommand(customerId, beneficiaryId);
        var deleteBeneficiaryResult = await _mediator.Send(command);
        
        return deleteBeneficiaryResult.Match<IActionResult>(
            success => Json(new { success = true }),
            errors => Json(new { success = false, message = errors.First().Description })
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
        
        modelState.Remove("CustomerBeneficiary.Name");
        modelState.Remove("CustomerBeneficiary.ApePaternal");
        modelState.Remove("CustomerBeneficiary.PhoneNumber");
        modelState.Remove("CustomerBeneficiary.Percentage");
        modelState.Remove("CustomerBeneficiary.Relationship");
    }
    
    private void RemoveValidationForChangeSecurityQuestions(ModelStateDictionary modelState)
    {
        modelState.Remove("ResetPassword.CurrentPassword");
        modelState.Remove("ResetPassword.Password");
        modelState.Remove("ResetPassword.ConfirmPassword");
        
        modelState.Remove("CustomerBeneficiary.Name");
        modelState.Remove("CustomerBeneficiary.ApePaternal");
        modelState.Remove("CustomerBeneficiary.PhoneNumber");
        modelState.Remove("CustomerBeneficiary.Percentage");
        modelState.Remove("CustomerBeneficiary.Relationship");
    }
    
    private void RemoveValidationForCreateBeneficiary(ModelStateDictionary modelState)
    {
        modelState.Remove("ResetPassword.CurrentPassword");
        modelState.Remove("ResetPassword.Password");
        modelState.Remove("ResetPassword.ConfirmPassword");
        
        modelState.Remove("SetSecurityQuestion.FirstQuestionId");
        modelState.Remove("SetSecurityQuestion.FirstQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecondQuestionId");
        modelState.Remove("SetSecurityQuestion.SecondQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionId");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecurityQuestions");
    }
    
    private void RemoveValidationForAssignBeneficiaryPercent(ModelStateDictionary modelState)
    {
        modelState.Remove("ResetPassword.CurrentPassword");
        modelState.Remove("ResetPassword.Password");
        modelState.Remove("ResetPassword.ConfirmPassword");
        
        modelState.Remove("SetSecurityQuestion.FirstQuestionId");
        modelState.Remove("SetSecurityQuestion.FirstQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecondQuestionId");
        modelState.Remove("SetSecurityQuestion.SecondQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionId");
        modelState.Remove("SetSecurityQuestion.ThirdQuestionAnswer");
        modelState.Remove("SetSecurityQuestion.SecurityQuestions");
        
        modelState.Remove("CustomerBeneficiary.Name");
        modelState.Remove("CustomerBeneficiary.ApePaternal");
        modelState.Remove("CustomerBeneficiary.PhoneNumber");
        modelState.Remove("CustomerBeneficiary.Percentage");
        modelState.Remove("CustomerBeneficiary.Relationship");
    }
}