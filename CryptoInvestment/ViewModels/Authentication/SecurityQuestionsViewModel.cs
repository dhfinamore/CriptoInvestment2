using System.ComponentModel.DataAnnotations;
using CryptoInvestment.Domain.SecurityQuestions;
using CryptoInvestment.ViewModels.DataValidation;

namespace CryptoInvestment.ViewModels.Authentication;

public class SecurityQuestionsViewModel
{
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Debe escoger una pregunta")]
    public int FirstQuestionId { get; set; }

    [Required(ErrorMessage = "Debe ingresar una respuesta")]
    public string FirstQuestionAnswer { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe escoger una pregunta")]
    public int SecondQuestionId { get; set; }
    
    [Required(ErrorMessage = "Debe ingresar una respuesta")]
    public string SecondQuestionAnswer { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe escoger una pregunta")]
    public int ThirdQuestionId { get; set; }
    
    [Required(ErrorMessage = "Debe ingresar una respuesta")]
    public string ThirdQuestionAnswer { get; set; } = null!;
    
    [UniqueSecurityQuestions]
    public List<SecurityQuestion> SecurityQuestions { get; set; } = [];
}