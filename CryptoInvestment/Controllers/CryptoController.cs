using CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;
using CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerRelationshipsQuery;
using CryptoInvestment.Application.CustomersPic.GetCustomerPicQuery;
using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
using CryptoInvestment.Domain.Customers;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.ViewModels.CustomerConfiguration;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoInvestment.Controllers;

[Authorize]
public class CryptoController : Controller
{
    private readonly ISender _mediator;

    public CryptoController(ISender mediator)
    {
        _mediator = mediator;
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
        
        return View(model);
    }
    
    public IActionResult Deposit()
    {
        return View();
    }
    
    public IActionResult Withdraw()
    {
        return View();
    }
    
    public IActionResult Movement()
    {
        return View();
    }

    public IActionResult Referral()
    {
        return View();
    }
}