using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersPic.DeleteCustomerPicCommand;

public record DeleteCustomerPicCommand(int CustomerId) : IRequest<ErrorOr<Deleted>>;