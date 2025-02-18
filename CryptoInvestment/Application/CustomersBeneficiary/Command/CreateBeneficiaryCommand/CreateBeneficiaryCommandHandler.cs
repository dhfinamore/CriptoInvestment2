using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.CreateBeneficiaryCommand;

public class CreateBeneficiaryCommandHandler : IRequestHandler<Command.CreateBeneficiaryCommand.CreateBeneficiaryCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerBeneficiaryRepository _beneficiaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBeneficiaryCommandHandler(
        ICustomerRepository customerRepository, 
        ICustomerBeneficiaryRepository beneficiaryRepository, 
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _beneficiaryRepository = beneficiaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(Command.CreateBeneficiaryCommand.CreateBeneficiaryCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(command.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        var customerBeneficiaries = await _beneficiaryRepository.GetBeneficiaryByCustomerIdAsync(command.CustomerId);
        
        if (command.BeneficiaryId is not null)
        {
            var beneficiary = customerBeneficiaries.FirstOrDefault(x => x.IdCustomerBeneficiary == command.BeneficiaryId);
            
            if (beneficiary is null)
                return Error.NotFound(description: "Beneficiary not found");
            
            beneficiary.Nombre = command.Name;
            beneficiary.ApePat = command.ApePaternal;
            beneficiary.ApeMat = command.ApeMaternal;
            beneficiary.Tel = command.PhoneNumber;
            beneficiary.RelationshipId = command.RelationshipId;
            
            await _beneficiaryRepository.UpdateCustomerBeneficiaryAsync(customerBeneficiaries);
            await _unitOfWork.CommitChangesAsync();
            
            return Result.Success;
        }
        
        var customerBeneficiary = new CustomerBeneficiary()
        {
            IdCustomer = command.CustomerId,
            Nombre = command.Name,
            ApePat = command.ApePaternal,
            ApeMat = command.ApeMaternal,
            Tel = command.PhoneNumber,
            RelationshipId = command.RelationshipId,
            Porcent = customerBeneficiaries.Count == 0 ? 100 : 0
        };
        
        await _beneficiaryRepository.AddBeneficiaryAsync(customerBeneficiary);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Success;
    }
}
