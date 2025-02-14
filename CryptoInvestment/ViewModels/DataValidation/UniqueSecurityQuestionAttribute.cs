using CryptoInvestment.ViewModels.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CryptoInvestment.ViewModels.DataValidation;

public class UniqueSecurityQuestionsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var model = (SecurityQuestionsViewModel)validationContext.ObjectInstance;

        var selectedQuestions = new[] 
        { 
            model.FirstQuestionId, 
            model.SecondQuestionId, 
            model.ThirdQuestionId 
        };

        if (selectedQuestions.GroupBy(q => q).Any(g => g.Count() > 1))
        {
            return new ValidationResult("Las preguntas de seguridad deben ser diferentes.");
        }

        return ValidationResult.Success!;
    }
}
