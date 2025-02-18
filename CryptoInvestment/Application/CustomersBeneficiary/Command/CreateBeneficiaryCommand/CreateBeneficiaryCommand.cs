using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.CreateBeneficiaryCommand;

public record CreateBeneficiaryCommand(
    int CustomerId,
    string Name,
    string ApePaternal,
    string? ApeMaternal,
    string PhoneNumber,
    int RelationshipId,
    int? BeneficiaryId = null) : IRequest<ErrorOr<Success>>;