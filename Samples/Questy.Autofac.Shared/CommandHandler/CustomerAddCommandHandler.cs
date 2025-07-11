using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Shared.Commands;
using Questy.Autofac.Shared.Entities;
using Questy.Autofac.Shared.Exceptions;
using Questy.Autofac.Shared.Notifications;
using Questy.Autofac.Shared.Repositories;

namespace Questy.Autofac.Shared.CommandHandler;

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