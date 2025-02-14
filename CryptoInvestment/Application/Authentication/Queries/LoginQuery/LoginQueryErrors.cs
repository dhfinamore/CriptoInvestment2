using ErrorOr;

namespace CryptoInvestment.Application.Authentication.Queries.LoginQuery;

public class LoginQueryErrors
{
    public static Error EmailNotVerified =>
        Error.Validation(
            code: "Authentication.EmailNotVerified",
            description: "El correo electrónico no ha sido verificado.");

    public static Error SecurityQuestionsNotConfigured =>
        Error.Validation(
            code: "Authentication.SecurityQuestionsNotConfigured",
            description: "Las preguntas de seguridad no han sido configuradas.");

    public static Error AccountLocked =>
        Error.Validation(
            code: "Authentication.AccountLocked",
            description: "Cuenta bloqueada por demasiados intentos fallidos. Inténtelo de nuevo en 15 minutos.");
}