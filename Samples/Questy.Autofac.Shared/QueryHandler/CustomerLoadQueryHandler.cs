using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Shared.Dto;
using Questy.Autofac.Shared.Exceptions;
using Questy.Autofac.Shared.Queries;
using Questy.Autofac.Shared.Repositories;

namespace Questy.Autofac.Shared.QueryHandler;

public class CustomerLoadQueryHandler : IRequestHandler<CustomerLoadQuery, CustomerDto>
{
    private readonly ICustomersRepository customersRepository;

    public CustomerLoadQueryHandler(ICustomersRepository customersRepository)
    {
        this.customersRepository = customersRepository;
    }

    public async Task<CustomerDto> Handle(CustomerLoadQuery request, CancellationToken cancellationToken)
    {
        var customer = this.customersRepository.FindCustomer(request.Id);

        if (customer == null)
        {
            throw new CustomerNotFoundException();
        }

        return await Task
            .FromResult(new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name
            })
            .ConfigureAwait(false);
    }
}