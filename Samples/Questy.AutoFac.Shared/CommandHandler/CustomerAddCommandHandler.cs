using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Shared.Commands;
using Questy.AutoFac.Shared.Entities;
using Questy.AutoFac.Shared.Exceptions;
using Questy.AutoFac.Shared.Notifications;
using Questy.AutoFac.Shared.Repositories;

namespace Questy.AutoFac.Shared.CommandHandler;

public class CustomerAddCommandHandler : IRequestHandler<CustomerAddCommand>
{
    private readonly ICustomersRepository customersRepository;
    private readonly IMediator mediator;

    public CustomerAddCommandHandler(ICustomersRepository customersRepository, IMediator mediator)
    {
        this.customersRepository = customersRepository;
        this.mediator = mediator;
    }

    public async Task Handle(CustomerAddCommand request, CancellationToken cancellationToken)
    {
        if (!this.customersRepository.AddCustomer(new Customer(request.Id, request.Name)))
        {
            throw new CustomerAlreadyExistsException();
        }

        await this.mediator
            .Publish(new CustomerAddedNotification(request.Name), cancellationToken)
            .ConfigureAwait(false);
    }
}