using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.DeleteBeneficiaryCommand;

public record DeleteBeneficiaryCommand(
    int CustomerId,
    int BeneficiaryId
) : IRequest<ErrorOr<Deleted>>;