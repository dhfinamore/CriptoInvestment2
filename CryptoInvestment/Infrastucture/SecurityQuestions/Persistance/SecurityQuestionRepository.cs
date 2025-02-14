using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.Infrastucture.Common;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.Infrastucture.SecurityQuestions.Persistance;

public class SecurityQuestionRepository : ISecurityQuestionRepository
{
    private readonly CryptoInvestmentDbContext _context;

    public SecurityQuestionRepository(CryptoInvestmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<SecurityQuestion>> GetSecurityQuestionsAsync()
    {
        return await _context.SecurityQuestions.ToListAsync();
    }
}