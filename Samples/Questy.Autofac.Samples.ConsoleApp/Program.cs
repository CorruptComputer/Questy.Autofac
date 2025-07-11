using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Questy.Autofac.Builder;
using Questy.Autofac.Shared.Commands;
using Questy.Autofac.Shared.Exceptions;
using Questy.Autofac.Shared.Queries;
using Questy.Autofac.Shared.Repositories;

namespace Questy.Autofac.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] _)
    {
        CancellationToken ctx = new CancellationToken();
        CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ctx);

        Console.CancelKeyPress += (x, y) =>
        {
            y.Cancel = true;
            cancellationTokenSource.Cancel(false);
        };

        ContainerBuilder builder = new ContainerBuilder();

        QuestyConfiguration configuration = QuestyConfigurationBuilder.Create(typeof(CustomerLoadQuery).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterQuesty(configuration);

        builder.RegisterType<CustomersRepository>()
            .As<ICustomersRepository>()
            .SingleInstance();

        IContainer container = builder.Build();

        ILifetimeScope lifetimeScope = container.Resolve<ILifetimeScope>();

        CustomerAddCommand googleCustomerAddCommand = new CustomerAddCommand(Guid.NewGuid(), "google");

        await using (ILifetimeScope scope = lifetimeScope.BeginLifetimeScope())
        {
            IMediator mediator = scope.Resolve<IMediator>();

            await mediator.Send(googleCustomerAddCommand, ctx);
        }

        await using (ILifetimeScope scope = lifetimeScope.BeginLifetimeScope())
        {
            IMediator mediator = scope.Resolve<IMediator>();

            Shared.Dto.CustomerDto customer = await mediator.Send(new CustomerLoadQuery(googleCustomerAddCommand.Id), ctx);

            Console.WriteLine(googleCustomerAddCommand.Name == customer.Name);

            try
            {
                await mediator.Send(new CustomerLoadQuery(Guid.Empty), ctx);
            }
            catch (CustomerNotFoundException)
            {
                Console.WriteLine("Expected that the customer could not be found bc we didn't add him b4.");
            }
        }

        Console.ReadKey();
    }
}