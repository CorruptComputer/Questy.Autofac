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
    private readonly ContainerBuilder builder;
    private IContainer container;

    public ContainerBuilderExtensionsTests()
    {
        this.builder = new ContainerBuilder();
    }
    
    [Fact]
    public void RegisterQuesty_RegistrationScopeScoped_InstancesSameInScope()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Scoped)
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var mediatorOne = this.container.Resolve<IMediator>();
        var mediatorTwo = this.container.Resolve<IMediator>();
        mediatorOne.ShouldBe(mediatorTwo);
    }
    
    [Fact]
    public void RegisterQuesty_RegistrationScopeTransient_InstancesNotSameInScope()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Transient)
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var mediatorOne = this.container.Resolve<IMediator>();
        var mediatorTwo = this.container.Resolve<IMediator>();
        mediatorOne.ShouldNotBe(mediatorTwo);
    }
    
    [Fact]
    public void RegisterQuesty_CustomMediatorProvided_ExpectInstances()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .UseMediatorType(typeof(CustomMediator))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var publisher = this.container.Resolve<IMediator>();
        publisher.ShouldBeTypeOf(typeof(CustomMediator));
    }
    
    [Fact]
    public void RegisterQuesty_CustomPublisherProvided_ExpectInstances()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .UseNotificationPublisher(typeof(CustomNotificationPublisher))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var publisher = this.container.Resolve<INotificationPublisher>();
        publisher.ShouldBeTypeOf(typeof(CustomNotificationPublisher));
    }

    [Fact]
    public void RegisterQuesty_ConfigurationProvided_ExpectInstances()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
    }
    
    [Fact]
    public void RegisterQuesty_ServiceProviderNotProvided_WrapperResolved()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var serviceProvider = this.container.Resolve<IServiceProvider>();
        serviceProvider.ShouldBeTypeOf(typeof(ServiceProviderWrapper));
    }
    
    [Fact]
    public void RegisterQuesty_ServiceProviderProvidedFromOutside_AutofacServiceProviderResolved()
    {
        this.builder.Populate(new ServiceCollection());
        
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        var serviceProvider = this.container.Resolve<IServiceProvider>();
        serviceProvider.ShouldBeTypeOf(typeof(AutofacServiceProvider));
    }
    
    [Fact]
    public void RegisterQuesty_Manual_ExpectInstances()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithRequestHandlersManuallyRegistered()
            .Build();

         this.builder
            .RegisterQuesty(configuration)
            .RegisterType<ResponseCommandHandler>()
            .As<IRequestHandler<ResponseCommand, Response>>();

         this.container = this.builder.Build();
        
         Assert.True(this.container.IsRegistered<IRequestHandler<ResponseCommand, Response>>(), "Responsehandler not registered");
    }
    
    [Fact]
    public void RegisterQuesty_ConfigurationProvidedWithCustomBehaviors_Resolvable()
    {
        var configuration = QuestyConfigurationBuilder
            .Create(typeof(ResponseCommand).Assembly)
            .WithCustomPipelineBehavior(typeof(LoggingBehavior<,>))
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        this.container = this.builder.RegisterQuesty(configuration).Build();
        
        this.AssertServiceRegistered();
        this.AssertServiceResolvable();
        this.container.IsRegistered(typeof(IPipelineBehavior<ResponseCommand, Response>));
        var behaviors = this.container.Resolve<IEnumerable<IPipelineBehavior<ResponseCommand, Response>>>();
        behaviors
            .Select(type => type.GetType())
            .ShouldContain(typeof(LoggingBehavior<ResponseCommand, Response>));
    }
    
    private void AssertServiceResolvable()
    {
        this.container.Resolve<IServiceProvider>();
        this.container.Resolve<IMediator>();
        this.container.Resolve<IRequestHandler<ResponseCommand, Response>>();
        this.container.Resolve<IRequestHandler<VoidCommand>>();
        this.container.Resolve<INotificationHandler<SampleNotification>>();
    }

    private void AssertServiceRegistered()
    {
        Assert.True(this.container.IsRegistered<IServiceProvider>(), "IServiceProvider not registered!");
        Assert.True(this.container.IsRegistered<INotificationPublisher>(), "INotificationPublisher not registered!");
        Assert.True(this.container.IsRegistered<IMediator>(), "Mediator not registered!");
        Assert.True(this.container.IsRegistered<ISender>(), "ISender not registered!");
        Assert.True(this.container.IsRegistered<IPublisher>(), "IPublisher not registered!");
        Assert.True(this.container.IsRegistered<IPipelineBehavior<ResponseCommand, Response>>(), "PiplineBehavior not registered");
        Assert.True(this.container.IsRegistered<IRequestHandler<ResponseCommand, Response>>(), "Responsehandler not registered");
        Assert.True(this.container.IsRegistered<IRequestHandler<VoidCommand>>(), "Voidhandler not registered");
        Assert.True(this.container.IsRegistered<IRequestPreProcessor<VoidCommand>>(), "Void Request Pre Processor not registered");
        Assert.True(this.container.IsRegistered<IRequestPostProcessor<VoidCommand, Unit>>(), "Void Request Post Processor not registered");
        Assert.True(this.container.IsRegistered<INotificationHandler<SampleNotification>>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (this.container is null)
        {
            return;
        }

        await this.container.DisposeAsync();
    }
}