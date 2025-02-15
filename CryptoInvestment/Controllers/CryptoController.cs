using CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;
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
        
        model.SetSecurityQuestion.SecurityQuestions = questions;
        
        return View(model);
    }
}