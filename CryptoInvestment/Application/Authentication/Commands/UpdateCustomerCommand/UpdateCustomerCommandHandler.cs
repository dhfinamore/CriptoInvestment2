using CryptoInvestment.Application.Common.Interface;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.UpdateCustomerCommand;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound("Customer not found");
        
        customer.Nombre = request.Name;
        customer.ApellidoPaterno = request.ApellidoPaterno;
        customer.ApellidoMaterno = request.ApellidoMaterno;
        customer.Phone = request.Phone;
        customer.ClPais = request.Country;
        customer.IdEstado = request.State;
        customer.City = request.City;
        
        if (request.BirthDate is not null)
        {
            try
            {
                DateTime date = DateTime.ParseExact(request.BirthDate, "MM/dd/yyyy", null);
                string formattedDate = date.ToString("yyyy-MM-dd");
                DateTime dateTime = DateTime.ParseExact(formattedDate, "yyyy-MM-dd", null);
                customer.FechaNacimiento = dateTime;
            }
            catch (Exception e)
            {
                return Error.Validation("Fecha de nacimiento invaldia");
            }
        }
        
        await _customerRepository.UpdateCustomerAsync(customer);
        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}