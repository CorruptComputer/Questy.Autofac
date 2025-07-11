using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Shouldly;
using Questy.Autofac.Builder;
using Questy.Autofac.Tests.Behaviors;
using Questy.Autofac.Tests.Commands;
using Questy.Autofac.Tests.CustomTypes;
using Questy.Autofac.Tests.Handler;
using Questy.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Questy.Autofac.Tests;

public class ContainerBuilderExtensionsTests : IAsyncLifetime
{
    private readonly ContainerBuilder builder = new();
    private IContainer? container;
    
    [Fact]
    public void RegisterQuesty_RegistrationScopeScoped_InstancesSameInScope()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Scoped)
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        IMediator mediatorOne = container.Resolve<IMediator>();
        IMediator mediatorTwo = container.Resolve<IMediator>();
        mediatorOne.ShouldBe(mediatorTwo);
    }
    
    [Fact]
    public void RegisterQuesty_RegistrationScopeTransient_InstancesNotSameInScope()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Transient)
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        IMediator mediatorOne = container.Resolve<IMediator>();
        IMediator mediatorTwo = container.Resolve<IMediator>();
        mediatorOne.ShouldNotBe(mediatorTwo);
    }
    
    [Fact]
    public void RegisterQuesty_CustomMediatorProvided_ExpectInstances()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .UseMediatorType(typeof(CustomMediator))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        IMediator publisher = container.Resolve<IMediator>();
        publisher.ShouldBeOfType<CustomMediator>();
    }
    
    [Fact]
    public void RegisterQuesty_CustomPublisherProvided_ExpectInstances()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .UseNotificationPublisher(typeof(CustomNotificationPublisher))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        INotificationPublisher publisher = container.Resolve<INotificationPublisher>();
        publisher.ShouldBeOfType<CustomNotificationPublisher>();
    }

    [Fact]
    public void RegisterQuesty_ConfigurationProvided_ExpectInstances()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
    }
    
    [Fact]
    public void RegisterQuesty_ServiceProviderNotProvided_WrapperResolved()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        IServiceProvider serviceProvider = container.Resolve<IServiceProvider>();
        serviceProvider.ShouldBeOfType<ServiceProviderWrapper>();
    }
    
    [Fact]
    public void RegisterQuesty_ServiceProviderProvidedFromOutside_AutofacServiceProviderResolved()
    {
        builder.Populate(new ServiceCollection());

        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        IServiceProvider serviceProvider = container.Resolve<IServiceProvider>();
        serviceProvider.ShouldBeOfType<AutofacServiceProvider>();
    }
    
    [Fact]
    public void RegisterQuesty_Manual_ExpectInstances()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithRequestHandlersManuallyRegistered()
            .Build();

         builder
            .RegisterQuesty(configuration)
            .RegisterType<ResponseCommandHandler>()
            .As<IRequestHandler<ResponseCommand, Response>>();

         container = builder.Build();
        
         Assert.True(container.IsRegistered<IRequestHandler<ResponseCommand, Response>>(), "Responsehandler not registered");
    }
    
    [Fact]
    public void RegisterQuesty_ConfigurationProvidedWithCustomBehaviors_Resolvable()
    {
        QuestyConfiguration configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithCustomPipelineBehavior(typeof(LoggingBehavior<,>))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        container = builder.RegisterQuesty(configuration).Build();
        
        AssertServiceRegistered();
        AssertServiceResolvable();
        container.IsRegistered(typeof(IPipelineBehavior<ResponseCommand, Response>));
        IEnumerable<IPipelineBehavior<ResponseCommand, Response>> behaviors = container.Resolve<IEnumerable<IPipelineBehavior<ResponseCommand, Response>>>();
        behaviors
            .Select(type => type.GetType())
            .ShouldContain(typeof(LoggingBehavior<ResponseCommand, Response>));
    }
    
    private void AssertServiceResolvable()
    {
        Assert.NotNull(container);

        container.Resolve<IServiceProvider>();
        container.Resolve<IMediator>();
        container.Resolve<IRequestHandler<ResponseCommand, Response>>();
        container.Resolve<IRequestHandler<VoidCommand>>();
        container.Resolve<INotificationHandler<SampleNotification>>();
    }

    private void AssertServiceRegistered()
    {
        Assert.NotNull(container);
        
        Assert.True(container.IsRegistered<IServiceProvider>(), "IServiceProvider not registered!");
        Assert.True(container.IsRegistered<INotificationPublisher>(), "INotificationPublisher not registered!");
        Assert.True(container.IsRegistered<IMediator>(), "Mediator not registered!");
        Assert.True(container.IsRegistered<ISender>(), "ISender not registered!");
        Assert.True(container.IsRegistered<IPublisher>(), "IPublisher not registered!");
        Assert.True(container.IsRegistered<IPipelineBehavior<ResponseCommand, Response>>(), "PiplineBehavior not registered");
        Assert.True(container.IsRegistered<IRequestHandler<ResponseCommand, Response>>(), "Responsehandler not registered");
        Assert.True(container.IsRegistered<IRequestHandler<VoidCommand>>(), "Voidhandler not registered");
        Assert.True(container.IsRegistered<IRequestPreProcessor<VoidCommand>>(), "Void Request Pre Processor not registered");
        Assert.True(container.IsRegistered<IRequestPostProcessor<VoidCommand, Unit>>(), "Void Request Post Processor not registered");
        Assert.True(container.IsRegistered<INotificationHandler<SampleNotification>>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (container is null)
        {
            return;
        }

        await container.DisposeAsync();
    }
}