using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;

public record SetSecurityQuestionCommand(
    int CustomerId,
    List<(int, string)> CustomerSecurityQuestions) : IRequest<ErrorOr<Success>>;
