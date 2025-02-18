using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;
using CryptoInvestment.Application.Authentication.Queries.GetCustomerByEmailQuery;
using CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Application.CustomersBeneficiary.Command.AssignPercentageCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Command.CreateBeneficiaryCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Command.DeleteBeneficiaryCommand;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerRelationshipsQuery;
using CryptoInvestment.Application.CustomersPic.DeleteCustomerPicCommand;
using CryptoInvestment.Application.CustomersPic.GetCustomerPicQuery;
using CryptoInvestment.Application.CustomersPic.SaveCustomerPicCommand;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.Services.ConfigurationModels;
using CryptoInvestment.ViewModels.CustomerConfiguration;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace CryptoInvestment.Controllers;

[Authorize]
public class CustomerConfigurationController : Controller
{
    private readonly ISender _mediator;
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;

    public CustomerConfigurationController(ISender mediator, IOptions<AppSettings> appSettings, IEmailService emailService)
    {
        _mediator = mediator;
        _emailService = emailService;
        _appSettings = appSettings.Value;
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        RemoveValidationForResetPassword(ModelState);

        if (!ModelState.IsValid)
        {
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
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
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
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
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

        var command = new CreateBeneficiaryCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerBeneficiary.Name,
            customerConfigurationViewModel.CustomerBeneficiary.ApePaternal,
            customerConfigurationViewModel.CustomerBeneficiary.ApeMaternal,
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber,
            customerConfigurationViewModel.CustomerBeneficiary.RelationshipId
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
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }

        var command = new CreateBeneficiaryCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerBeneficiary.Name,
            customerConfigurationViewModel.CustomerBeneficiary.ApePaternal,
            customerConfigurationViewModel.CustomerBeneficiary.ApeMaternal,
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber,
            customerConfigurationViewModel.CustomerBeneficiary.RelationshipId,
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
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);

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
    
    [HttpPost]
    public IActionResult UpdateImages(string documentType, int customerId)
    {
        string type = documentType switch
        {
            "passport" => "Pasaporte",
            "license" => "Licencia",
            _ => "Identificacion"
        };

        var model = new UpdateImageViewModel
        {
            CustomerId = customerId,
            RequiresTwoPhotos = documentType != "passport",
            Type = type
        };
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveCustomerPic(UpdateImageViewModel updateImageViewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Error al guardar la imagen.");
        }

        var command = new SaveCustomerPicCommand(
            updateImageViewModel.CustomerId,
            updateImageViewModel.Type,
            updateImageViewModel.PictureFrontBase64,
            updateImageViewModel.PictureBackBase64
        );

        var saveCustomerPicResult = await _mediator.Send(command);

        return saveCustomerPicResult.Match<IActionResult>(
            success => Json(new { success = true }),
            errors => Json(new { success = false, message = errors.First().Description })
        );
    }

    public async Task<IActionResult> SendDocumentsToValidation(int token)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Authentication");
        
        var query = new GetCustomerByEmailQuery(email!);
        var getCustomerByEmailResult = await _mediator.Send(query);
        
        var customerId = getCustomerByEmailResult.Match<int>(
            customer => customer.IdCustomer,
            _ => 0
        );
        
        if (customerId == 0)
            return RedirectToAction("Login", "Authentication");

        if (customerId != token)
            return BadRequest("No tiene permisos para realizar esta acción.");
        
        var body = await CreateDocumentsValidationEmail(email);
        var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(_appSettings.AdminDestination, "Solicitud de Validacion de documentos", body);

        return RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "subir-documentos" });
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCustomerPic(int customerId)
    {
        var command = new DeleteCustomerPicCommand(customerId);
        var deleteCustomerPicResult = await _mediator.Send(command);

        return deleteCustomerPicResult.Match<IActionResult>(
            success => Json(new { success = true }),
            errors => Json(new { success = false, message = errors.First().Description })
        );
    }
    
    private async Task LoadCustomerConfigurationViewModel(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        var query = new ListSecurityQuestionsQuery();
        var listSecurityQuestionsResult = await _mediator.Send(query);
        
        var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
            questions => questions,
            _ => null!
        );
        
        var query2 = new GetCustomerSecurityQuestionsQuery(customerConfigurationViewModel.CustomerId);
        var getCustomerSecurityQuestionsResult = await _mediator.Send(query2);
        
        var customerQuestions = getCustomerSecurityQuestionsResult.Match<List<CustomerQuestion>>(
            customerQuestions => customerQuestions,
            _ => null!
        );
        
        customerConfigurationViewModel.SetSecurityQuestion.SecurityQuestions = questions;
        customerConfigurationViewModel.SetSecurityQuestion.FirstQuestionId = customerQuestions[0].IdQuestion;
        customerConfigurationViewModel.SetSecurityQuestion.SecondQuestionId = customerQuestions[1].IdQuestion;
        customerConfigurationViewModel.SetSecurityQuestion.ThirdQuestionId = customerQuestions[2].IdQuestion;
        
        var query3 = new GetCustomerBeneficiariesQuery(customerConfigurationViewModel.CustomerId);
        var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
        var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
            customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
            _ => null!
        );

        customerConfigurationViewModel.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
        
        var query5 = new GetCustomerRelationshipQuery();
        var getCustomerRelationshipResult = await _mediator.Send(query5);
        
        var customerRelationship = getCustomerRelationshipResult.Match<List<CustomerRelationship>>(
            customerRelationship => customerRelationship,
            _ => null!
        );
        
        customerConfigurationViewModel.CustomerBeneficiary.CustomerRelationships = customerRelationship;
        
        var query4 = new GetCustomerPicQuery(customerConfigurationViewModel.CustomerId);
        var getCustomerPicResult = await _mediator.Send(query4);
        
        var customerPic = getCustomerPicResult.Match<CustomerPic>(
            customerPic => customerPic,
            _ => null
        );
        
        customerConfigurationViewModel.CustomerPic = customerPic;
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
    
    private async Task<string> CreateDocumentsValidationEmail(string email)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "SendDocumentsToValidation.html");

        string bodyTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

        string body = bodyTemplate
            .Replace("{email}", email);

        return body;
    }
}
