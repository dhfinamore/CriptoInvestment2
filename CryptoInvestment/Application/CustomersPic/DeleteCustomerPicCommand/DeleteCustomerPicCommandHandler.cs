using CryptoInvestment.Application.Common.Interface;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.CustomersPic.DeleteCustomerPicCommand;

public class DeleteCustomerPicCommandHandler : IRequestHandler<DeleteCustomerPicCommand, ErrorOr<Deleted>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerPicCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteCustomerPicCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        var customerPic = await _customerRepository.GetCustomerPic(customer.IdCustomer);
        
        if (customerPic is null)
            return Error.NotFound(description: "Customer pic not found");
        
        await _customerRepository.DeleteCustomerPic(customerPic);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}