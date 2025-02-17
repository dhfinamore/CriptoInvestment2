using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersPic.SaveCustomerPicCommand;

public record SaveCustomerPicCommand(
    int CustomerId,
    string? Type,
    string? PictureFrontBase64,
    string? PictureBackBase64
) : IRequest<ErrorOr<CustomerPic>>;