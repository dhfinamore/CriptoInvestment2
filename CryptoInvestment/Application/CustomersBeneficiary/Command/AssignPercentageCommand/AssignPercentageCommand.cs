using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.AssignPercentageCommand;

public record AssignPercentageCommand(
    int CustomerId,
    List<CustomerBeneficiary> CustomerBeneficiaries) : IRequest<ErrorOr<Success>>;