using CryptoInvestment.Application.Authentication.Commands.CreateCustomerWalletsCommand;
using CryptoInvestment.Application.Authentication.Commands.DeleteCustomersWallet;
using CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;
using CryptoInvestment.Application.Authentication.Commands.SetWithdrawalsPasswordCommand;
using CryptoInvestment.Application.Authentication.Commands.UpdateCustomerCommand;
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
using CryptoInvestment.Application.CustomerWithdrawalWallets.Queries;
using CryptoInvestment.Application.InvOperations.Queries.ListInvCurrenciesQuery;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.Infrastucture.Common;
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
    // TODO Remove this
    private readonly CryptoInvestmentDbContext _context;

    public CustomerConfigurationController(ISender mediator, IOptions<AppSettings> appSettings, IEmailService emailService, CryptoInvestmentDbContext context)
    {
        _mediator = mediator;
        _emailService = emailService;
        _context = context;
        _appSettings = appSettings.Value;
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);

        if (!ModelState.IsValid)
        {
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
            TempData["activeTab"] = "change-password";
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
                
                TempData["ActiveTab"] = "change-password";
                return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
            });
    }
    
    [HttpPost]
    public async Task<IActionResult> ChangeSecurityQuestions(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);

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
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);

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
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber!,
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
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);

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
            customerConfigurationViewModel.CustomerBeneficiary.PhoneNumber!,
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
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);

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
    public async Task<IActionResult> CreateWithdrawalWallet(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);

        if (!ModelState.IsValid)
        {
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);

            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }
        
        var command = new CreateCustomerWalletCommand(
            customerConfigurationViewModel.CustomerId, 
            customerConfigurationViewModel.CustomerWithdrawalWallet.WalletName,
            customerConfigurationViewModel.CustomerWithdrawalWallet.InvCurrency,
            customerConfigurationViewModel.CustomerWithdrawalWallet.WalletAccount,
            customerConfigurationViewModel.CustomerWithdrawalWallet.Used,
            customerConfigurationViewModel.CustomerWithdrawalWallet.WalletId);

        var upsertWalletResult = await _mediator.Send(command);
        
        return upsertWalletResult.Match<IActionResult>(
            success => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "withdrawals" }),
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
    public IActionResult UpdateImages(string documentType, int customerId, string method)
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
            Type = type,
            IsCamera = method == "camera"
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
        
        var customer = getCustomerByEmailResult.Match<Customer>(
            customer => customer,
            _ => null!
        );
        
        if (customer is null)
            return RedirectToAction("Login", "Authentication");

        if (customer.IdCustomer != token)
            return BadRequest("No tiene permisos para realizar esta acción.");
        
        var body = await CreateDocumentsValidationEmail(email);
        var sendVerificationEmailResult = await _emailService.SendVerificationEmailAsync(_appSettings.AdminDestination, "Solicitud de Validacion de documentos", body);

        // TODO Move to aplication layer
        customer.DocsValidated = 1;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        
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
    
    [HttpPost]
    public async Task<IActionResult> SetWithdrawalsPassword(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);
        
        if (!ModelState.IsValid)
        {
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }
        
        var command = new SetWithdrawalsPasswordCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.WithdrawalsPassword.Character1 +
            customerConfigurationViewModel.WithdrawalsPassword.Character2 +
            customerConfigurationViewModel.WithdrawalsPassword.Character3 +
            customerConfigurationViewModel.WithdrawalsPassword.Character4 +
            customerConfigurationViewModel.WithdrawalsPassword.Character5 +
            customerConfigurationViewModel.WithdrawalsPassword.Character6
        );
        
        var setWithdrawalsPasswordResult = await _mediator.Send(command);
        
        return setWithdrawalsPasswordResult.Match<IActionResult>(
            success => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "withdrawals" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
            }
        );
    }
    
    private async Task LoadCustomerConfigurationViewModel(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            customerConfigurationViewModel.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        
        var email = HttpContext.Session.GetString("UserEmail");
        var query1 = new GetCustomerByEmailQuery(email!);

        var customer = await _mediator.Send(query1);
        var customerResult = customer.Match(
            c => c,
            _ => null!
        );

        customerConfigurationViewModel.CustomerInformation = new CustomerInformationViewModel()
        {
            Name = customerResult.Nombre!,
            ApellidoPaterno = customerResult.ApellidoPaterno!,
            ApellidoMaterno = customerResult.ApellidoMaterno,
            Phone = customerResult.Phone!,
            Coutry = customerResult.ClPais,
            State = customerResult.IdEstado,
            City = customerResult.City,
            BirthDate = customerResult.FechaNacimiento?.ToString("MM/dd/yyyy")
        };
        
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

        var query6 = new ListCustomerWithdrawalWalletsQuery(customerConfigurationViewModel.CustomerId);
        var listCustomerWalletResult = await _mediator.Send(query6);

        var wallets = listCustomerWalletResult.Match<List<CustomerWithdrawalWallet>>(
            wallets => wallets,
            _ => null! );

        customerConfigurationViewModel.CustomerWithdrawalWallets = wallets;
        
        var query7 = new ListInvCurrenciesQuery();
        var getInvCurrenciesResult = await _mediator.Send(query7);
        
        var currencies = getInvCurrenciesResult.Match<List<InvCurrency>>(
            currencies => currencies,
            _ => null!
        );
        
        customerConfigurationViewModel.CustomerWithdrawalWallet.InvCurrencies = currencies;
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerWallet(int customerId, int walletId)
    {
        var command = new DeleteCustomerWalletCommand(customerId, walletId);
        var deleteWalletResult = await _mediator.Send(command);
        
        return deleteWalletResult.Match<IActionResult>(
            success => Json(new { success = true }),
            errors => Json(new { success = false, message = errors.First().Description })
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveUserConfigurations(CustomerConfigurationViewModel customerConfigurationViewModel)
    {
        IgnoreValidationForCreateBeneficiary(ModelState);
        IgnoreValidationForSecurityQuestions(ModelState);
        IgnoreValidationForWithdrawalPassword(ModelState);
        IgnoreValidationForUserConfiguration(ModelState);
        IgnoreValidationForResetPassword(ModelState);
        
        if (!ModelState.IsValid)
        {
            await LoadCustomerConfigurationViewModel(customerConfigurationViewModel);
            return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
        }
        
        var command = new UpdateCustomerCommand(
            customerConfigurationViewModel.CustomerId,
            customerConfigurationViewModel.CustomerInformation.Name,
            customerConfigurationViewModel.CustomerInformation.ApellidoPaterno,
            customerConfigurationViewModel.CustomerInformation.ApellidoMaterno,
            customerConfigurationViewModel.CustomerInformation.Phone,
            customerConfigurationViewModel.CustomerInformation.Coutry,
            customerConfigurationViewModel.CustomerInformation.State,
            customerConfigurationViewModel.CustomerInformation.City,
            customerConfigurationViewModel.CustomerInformation.BirthDate
        );
        
        var updateCustomerResult = await _mediator.Send(command);
        
        return updateCustomerResult.Match<IActionResult>(
            success => RedirectToAction("CustomerConfiguration", "Crypto", new { activeTab = "general" }),
            errors =>
            {
                ModelState.AddModelError("", errors.First().Description);
                return View("~/Views/Crypto/CustomerConfiguration.cshtml", customerConfigurationViewModel);
            }
        );
    }

    private void IgnoreValidationForResetPassword(ModelStateDictionary modelStateDictionary)
    {
        modelStateDictionary.Remove("ResetPassword.CurrentPassword");
        modelStateDictionary.Remove("ResetPassword.Password");
        modelStateDictionary.Remove("ResetPassword.ConfirmPassword");
    }
    
    private void IgnoreValidationForSecurityQuestions(ModelStateDictionary modelStateDictionary)
    {
        modelStateDictionary.Remove("SetSecurityQuestion.FirstQuestionId");
        modelStateDictionary.Remove("SetSecurityQuestion.FirstQuestionAnswer");
        modelStateDictionary.Remove("SetSecurityQuestion.SecondQuestionId");
        modelStateDictionary.Remove("SetSecurityQuestion.SecondQuestionAnswer");
        modelStateDictionary.Remove("SetSecurityQuestion.ThirdQuestionId");
        modelStateDictionary.Remove("SetSecurityQuestion.ThirdQuestionAnswer");
        modelStateDictionary.Remove("SetSecurityQuestion.SecurityQuestions");
    }
    
    private void IgnoreValidationForCreateBeneficiary(ModelStateDictionary modelStateDictionary)
    {
        modelStateDictionary.Remove("CustomerBeneficiary.Name");
        modelStateDictionary.Remove("CustomerBeneficiary.ApePaternal");
        modelStateDictionary.Remove("CustomerBeneficiary.PhoneNumber");
        modelStateDictionary.Remove("CustomerBeneficiary.Percentage");
        modelStateDictionary.Remove("CustomerBeneficiary.Relationship");
    }
    
    private void IgnoreValidationForWithdrawalPassword(ModelStateDictionary modelStateDictionary)
    {
        modelStateDictionary.Remove("WithdrawalsPassword.Character1");
        modelStateDictionary.Remove("WithdrawalsPassword.Character2");
        modelStateDictionary.Remove("WithdrawalsPassword.Character3");
        modelStateDictionary.Remove("WithdrawalsPassword.Character4");
        modelStateDictionary.Remove("WithdrawalsPassword.Character5");
        modelStateDictionary.Remove("WithdrawalsPassword.Character6");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter1");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter2");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter3");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter4");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter5");
        modelStateDictionary.Remove("WithdrawalsPassword.ConfirmCharacter6");
    }
    
    private void IgnoreValidationForUserConfiguration(ModelStateDictionary modelStateDictionary)
    {
        modelStateDictionary.Remove("CustomerInformation.Name");
        modelStateDictionary.Remove("CustomerInformation.ApellidoPaterno");
        modelStateDictionary.Remove("CustomerInformation.Phone");
        modelStateDictionary.Remove("CustomerInformation.Coutry");
        modelStateDictionary.Remove("CustomerInformation.State");
        modelStateDictionary.Remove("CustomerInformation.City");
        modelStateDictionary.Remove("CustomerInformation.BirthDate");
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
