using CryptoInvestment.Application.Authentication.Queries.GetCustomerByEmailQuery;
using CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;
using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerRelationshipsQuery;
using CryptoInvestment.Application.CustomersPic.GetCustomerPicQuery;
using CryptoInvestment.Application.CustomerWithdrawalWallets.Queries;
using CryptoInvestment.Application.InvAssets.Commands;
using CryptoInvestment.Application.InvAssets.Queries;
using CryptoInvestment.Application.InvOperations.Command;
using CryptoInvestment.Application.InvOperations.Queries.ListInvActionsQuery;
using CryptoInvestment.Application.InvOperations.Queries.ListInvCurrenciesQuery;
using CryptoInvestment.Application.InvOperations.Queries.ListInvOperationsQuery;
using CryptoInvestment.Application.InvPlans.Queries;
using CryptoInvestment.Application.InvPlans.Queries.ListInvPlanQuery;
using CryptoInvestment.Application.Referrals.Commands;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.InvPlans;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.Infrastucture.Common;
using CryptoInvestment.ViewModels.CustomerConfiguration;
using CryptoInvestment.ViewModels.Deposit;
using CryptoInvestment.ViewModels.Movements;
using CryptoInvestment.ViewModels.Referrals;
using CryptoInvestment.ViewModels.Withdraw;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Controllers;

[Authorize]
public class CryptoController : Controller
{
    private readonly ISender _mediator;
    private readonly IEncryptionService _encryptionService;
    private readonly CryptoInvestmentDbContext _context;

    public CryptoController(ISender mediator, IEncryptionService encryptionService, CryptoInvestmentDbContext context)
    {
        _mediator = mediator;
        _encryptionService = encryptionService;
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }
    
    public async Task<IActionResult> CustomerConfiguration()
    {
        var model = new CustomerConfigurationViewModel();
            
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
        
        var email = HttpContext.Session.GetString("UserEmail");
        var query1 = new GetCustomerByEmailQuery(email!);

        var customer = await _mediator.Send(query1);
        var customerResult = customer.Match(
            c => c,
            _ => null!
        );
        
        if (customerResult is null)
            return RedirectToAction("Login", "Authentication");

        model.CustomerInformation = new CustomerInformationViewModel()
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
        
        model.DocsValidated = customer.Match(
            c => c.DocsValidated,
            _ => 0
        );
        
        model.IsWithdrawalsPasswordSet = customer.Match(
            c => c.PasswdWithdrawal != null,
            _ => false
        );
        
        var query = new ListSecurityQuestionsQuery();
        var listSecurityQuestionsResult = await _mediator.Send(query);
        
        var questions = listSecurityQuestionsResult.Match<List<SecurityQuestion>>(
            questions => questions,
            _ => null!
        );
        
        var query2 = new GetCustomerSecurityQuestionsQuery(model.CustomerId);
        var getCustomerSecurityQuestionsResult = await _mediator.Send(query2);
        
        var customerQuestions = getCustomerSecurityQuestionsResult.Match<List<CustomerQuestion>>(
            customerQuestions => customerQuestions,
            _ => null!
        );
        
        model.SetSecurityQuestion.SecurityQuestions = questions;
        model.SetSecurityQuestion.FirstQuestionId = customerQuestions[0].IdQuestion;
        model.SetSecurityQuestion.SecondQuestionId = customerQuestions[1].IdQuestion;
        model.SetSecurityQuestion.ThirdQuestionId = customerQuestions[2].IdQuestion;
        
        var query3 = new GetCustomerBeneficiariesQuery(model.CustomerId);
        var getCustomerBeneficiariesResult = await _mediator.Send(query3);
        
        var customerBeneficiaries = getCustomerBeneficiariesResult.Match<List<CustomerBeneficiary>>(
            customerBeneficiaries => customerBeneficiaries.Count > 0 ? customerBeneficiaries : [],
            _ => null!
        );

        model.CustomerBeneficiary.CustomerBeneficiaries = customerBeneficiaries;
        
        var query5 = new GetCustomerRelationshipQuery();
        var getCustomerRelationshipResult = await _mediator.Send(query5);
        
        var customerRelationship = getCustomerRelationshipResult.Match<List<CustomerRelationship>>(
            customerRelationship => customerRelationship,
            _ => null!
        );
        
        model.CustomerBeneficiary.CustomerRelationships = customerRelationship;
        
        var query4 = new GetCustomerPicQuery(model.CustomerId);
        var getCustomerPicResult = await _mediator.Send(query4);
        
        var customerPic = getCustomerPicResult.Match<CustomerPic>(
            customerPic => customerPic,
            _ => null
        );
        
        model.CustomerPic = customerPic;

        var query6 = new ListCustomerWithdrawalWalletsQuery(model.CustomerId);
        var listCustomerWalletResult = await _mediator.Send(query6);

        var wallets = listCustomerWalletResult.Match<List<CustomerWithdrawalWallet>>(
            wallets => wallets,
            _ => null! );

        model.CustomerWithdrawalWallets = wallets;
        
        var query7 = new ListInvCurrenciesQuery();
        var getInvCurrenciesResult = await _mediator.Send(query7);
        
        var currencies = getInvCurrenciesResult.Match<List<InvCurrency>>(
            currencies => currencies,
            _ => null!
        );
        
        model.CustomerWithdrawalWallet.InvCurrencies = currencies;
        
        return View(model);
    }
    
    public async Task<IActionResult> Deposit()
    {
        var model = new InvPlanViewModel();
            
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }

        var query = new ListInvPlansQuery(model.CustomerId);
        var getInvPlansResult = await _mediator.Send(query);
        
        var invPlans = getInvPlansResult.Match<List<InvPlan>>(
            invPlans => invPlans,
            _ => null!
        );
        
        model.InvPlans = invPlans;
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DepositWizard(int customerId, int invPlanId)
    {
        var query = new ListInvPlansQuery(customerId);
        var getInvPlansResult = await _mediator.Send(query);
        
        var invPlans = getInvPlansResult.Match<List<InvPlan>>(
            invPlans => invPlans,
            _ => null!
        );
        
        var query2 = new ListInvCurrenciesQuery();
        var getInvCurrenciesResult = await _mediator.Send(query2);
        
        var currencies = getInvCurrenciesResult.Match<List<InvCurrency>>(
            currencies => currencies,
            _ => null!
        );
        
        var query3 = new GetCustomerBalanceQuery(customerId);
        var getCustomerBalanceResult = await _mediator.Send(query3);
        
        var balances = getCustomerBalanceResult.Match<List<InvBalance>>(
            balances => balances,
            _ => null!
        );
        
        var model = new DepositViewModel()
        {
            CustomerId = customerId,
            InvPlanId = invPlanId,
            InvPlans = invPlans,
            InvCurrencies = currencies,
            ReinversionAmount = 0,
            InvBalances = balances
        };
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlan(DepositViewModel depositViewModel)
    {
        if (!ModelState.IsValid)
        {
            var query = new ListInvPlansQuery(depositViewModel.CustomerId);
            var getInvPlansResult = await _mediator.Send(query);
        
            depositViewModel.InvPlans = getInvPlansResult.Match<List<InvPlan>>(
                invPlans => invPlans,
                _ => null!
            );
        
            var query2 = new ListInvCurrenciesQuery();
            var getInvCurrenciesResult = await _mediator.Send(query2);
        
            depositViewModel.InvCurrencies = getInvCurrenciesResult.Match<List<InvCurrency>>(
                currencies => currencies,
                _ => null!
            );
        
            var query3 = new GetCustomerBalanceQuery(depositViewModel.CustomerId);
            var getCustomerBalanceResult = await _mediator.Send(query3);
        
            depositViewModel.InvBalances = getCustomerBalanceResult.Match<List<InvBalance>>(
                balances => balances,
                _ => null!
            );
            
            return View("~/Views/Crypto/DepositWizard.cshtml", depositViewModel);
        }
        
        var command = new CreateInvAssetsCommand(
            depositViewModel.CustomerId,
            depositViewModel.SelectedCurrencyId,
            depositViewModel.InvPlanId,
            depositViewModel.DepositAmount + depositViewModel.ReinversionAmount,
            depositViewModel.ReinversionAmount,
            depositViewModel.EndType,
            depositViewModel.ReinvestPercent,
            depositViewModel.MonthProfit
        );

        var createInvAssetsResult = await _mediator.Send(command);

        return RedirectToAction(createInvAssetsResult.IsError ? "Deposit" : "Dashboard", "Crypto");
    }
    
    public async Task<IActionResult> Withdraw()
    {
        var model = new WithdrawViewModel();
        
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
        
        //TODO MOVE this
        var balance = await _context.InvBalances
            .Where(b => b.IdCustomer == model.CustomerId)
            .ToListAsync();
        
        model.InvBalances = balance;
        
        var query = new ListCustomerWithdrawalWalletsQuery(model.CustomerId);
        var listCustomerWalletResult = await _mediator.Send(query);
        
        var wallets = listCustomerWalletResult.Match<List<CustomerWithdrawalWallet>>(
            wallets => wallets,
            _ => null! );
        
        model.CustomerWithdrawalWallets = wallets;
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Withdraw(int balanceId, decimal amount, int walletId)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Dashboard", "Crypto");

        var command = new CreateWithdrawCommand(balanceId, amount, walletId);
        var createWithdrawResult = await _mediator.Send(command);
        
        return createWithdrawResult.Match<IActionResult>(
            success => Json(new { success = true }),
            errors => Json(new { success = false, message = errors.First().Description })
        );
    }
    
    public async Task<IActionResult> Movement()
    {
        var model = new MovementsViewModel();
        
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
        
        var query = new ListInvActionsQuery();
        var getInvActionsResult = await _mediator.Send(query);
        
        var invActions = getInvActionsResult.Match<List<InvAction>>(
            invActions => invActions,
            _ => null!
        );
        
        var query2 = new ListInvOperationsQuery(model.CustomerId);
        var getInvOperationsResult = await _mediator.Send(query2);
        
        var invOperations = getInvOperationsResult.Match<List<InvOperation>>(
            invOperations => invOperations,
            _ => null!
        );

        var query3 = new ListInvPlansQuery();
        var getInvPlansResult = await _mediator.Send(query3);
        
        var invPlans = getInvPlansResult.Match<List<InvPlan>>(
            invPlans => invPlans,
            _ => null!
        );
        
        model.InvActions = invActions;
        model.InvOperations = invOperations;
        model.InvPlans = invPlans;
        
        return View(model);
    }

    public async Task<IActionResult> Referral()
    {
        var model = new ReferralViewModel();
        
        if (HttpContext.User.Identity!.IsAuthenticated)
        {
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId")!);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
        
        var query = new GetReferralsCommand(model.CustomerId);
        var getReferralsResult = await _mediator.Send(query);
        
        var referrals = getReferralsResult.Match<List<Referral>>(
            referrals => referrals.ConvertAll(referral => new Referral()
            {
                Name = $"{referral.Nombre} {referral.ApellidoPaterno}",
                Email = referral.Email,
                PhoneNumber = referral.Phone!
            }),
            _ => []
        );
        
        model.ReferralList = referrals;
        model.ReferralCode = GenerateReferralLink(model.CustomerId);

        return View(model);
    }
    
    public string GenerateReferralLink(int customerId)
    {
        string referralToken = _encryptionService.EncryptId(customerId.ToString());
        string referralUrl = Url.Action("Register", "Authentication", new { token = referralToken }, Request.Scheme)!;
        return referralUrl;
    }
}