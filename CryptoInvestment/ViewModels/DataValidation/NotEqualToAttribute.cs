using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CryptoInvestment.ViewModels.DataValidation;

public class NotEqualToAttribute : ValidationAttribute
{
    public string OtherProperty { get; }

    public NotEqualToAttribute(string otherProperty)
    {
        OtherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        PropertyInfo? otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
        if (otherPropertyInfo == null)
        {
            return new ValidationResult($"La propiedad '{OtherProperty}' no existe.");
        }

        var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        
        return Equals(value, otherValue) ? 
            new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} debe ser distinto de {OtherProperty}.") : 
            ValidationResult.Success;
    }
}
