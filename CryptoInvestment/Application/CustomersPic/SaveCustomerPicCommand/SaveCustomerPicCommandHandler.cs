using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersPic.SaveCustomerPicCommand;

public class SaveCustomerPicCommandHandler : IRequestHandler<SaveCustomerPicCommand, ErrorOr<CustomerPic>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveCustomerPicCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CustomerPic>> Handle(SaveCustomerPicCommand command, CancellationToken cancellationToken)
    {
        byte[]? frontBytes = null;
        byte[]? backBytes = null;

        if (!string.IsNullOrEmpty(command.PictureFrontBase64))
        {
            frontBytes = Convert.FromBase64String(ExtractBase64(command.PictureFrontBase64));
        }
        if (!string.IsNullOrEmpty(command.PictureBackBase64))
        {
            backBytes = Convert.FromBase64String(ExtractBase64(command.PictureBackBase64));
        }

        var customerPic = await _customerRepository.GetCustomerPic(command.CustomerId);

        if (customerPic == null)
        {
            customerPic = new CustomerPic
            {
                IdCustomer = command.CustomerId,
                Type = command.Type,
                PictureFront = frontBytes,
                PictureBack = backBytes
            };
            await _customerRepository.CreateCustomerPicAsync(customerPic);
        }
        else
        {
            customerPic.Type = command.Type;
            customerPic.PictureFront = frontBytes;
            customerPic.PictureBack = backBytes;
            await _customerRepository.UpdateCustomerPic(customerPic);
        }

        await _unitOfWork.CommitChangesAsync();
        return customerPic;
    }
    
    private string ExtractBase64(string dataUrl)
    {
        var commaIndex = dataUrl.IndexOf(',');
        return commaIndex >= 0 ? dataUrl[(commaIndex + 1)..] : dataUrl;
    }
}