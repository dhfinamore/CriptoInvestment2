using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.CreateBeneficiaryCommand;

public record CreateBeneficiaryCommand(
    int CustomerId,
    string Name,
    string ApePaternal,
    string? ApeMaternal,
    string PhoneNumber,
    string Relationship,
    int? BeneficiaryId = null) : IRequest<ErrorOr<Success>>;