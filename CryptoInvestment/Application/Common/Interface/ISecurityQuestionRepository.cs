using CryptoInvestment.Domain.SecurityQuestions;

namespace CryptoInvestment.Application.Common.Interface;

public interface ISecurityQuestionRepository
{
    public Task<List<SecurityQuestion>> GetSecurityQuestionsAsync();
}