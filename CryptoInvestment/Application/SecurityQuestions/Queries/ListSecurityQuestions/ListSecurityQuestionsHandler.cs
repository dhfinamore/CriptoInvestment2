using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.SecurityQuestions;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;

public class ListSecurityQuestionsHandler : IRequestHandler<ListSecurityQuestionsQuery, ErrorOr<List<SecurityQuestion>>>
{
    private readonly ISecurityQuestionRepository _securityQuestionRepository;

    public ListSecurityQuestionsHandler(ISecurityQuestionRepository securityQuestionRepository)
    {
        _securityQuestionRepository = securityQuestionRepository;
    }

    public async Task<ErrorOr<List<SecurityQuestion>>> Handle(ListSecurityQuestionsQuery request, CancellationToken cancellationToken)
    {
        var securityQuestions = await _securityQuestionRepository.GetSecurityQuestionsAsync();
        return securityQuestions.Count > 0 ? 
            securityQuestions : 
            Error.NotFound(description: "No security questions found");
    }
}
