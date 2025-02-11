namespace CryptoInvestment.Application.Authentication.Commands.RegisterCommand;

using ErrorOr;

public static class RegisterCommandErrors
{
    public static Error EmailAlreadyRegistered =>
        Error.Conflict(
            code: "Register.EmailAlreadyRegistered",
            description: "El correo ya está registrado.");

    public static Error PhoneAlreadyRegistered =>
        Error.Conflict(
            code: "Register.PhoneAlreadyRegistered",
            description: "El número de teléfono ya está registrado.");
}
