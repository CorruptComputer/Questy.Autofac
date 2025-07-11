using System.Reflection;
using Autofac;
using Questy.NotificationPublishers;

namespace Questy.AutoFac.Builder;

public class QuestyConfigurationBuilder
{
    private readonly Assembly[] handlersFromAssembly;

    private Type mediatorType = typeof(Mediator);
    private Type notificationPublisherType = typeof(ForeachAwaitPublisher);
    
    private readonly HashSet<Type> internalCustomPipelineBehaviorTypes = new();
    private readonly HashSet<Type> internalCustomStreamPipelineBehaviorTypes = new();
    private readonly HashSet<Type> internalOpenGenericHandlerTypesToRegister = new();

    private RegistrationScope registrationScope = RegistrationScope.Transient;

    private QuestyConfigurationBuilder(Assembly[] handlersFromAssembly)
    {
        if (handlersFromAssembly == null || !handlersFromAssembly.Any() || handlersFromAssembly.All(x => x == null))
        {
            throw new ArgumentNullException(nameof(handlersFromAssembly),
                $"Must provide assemblies in order to request {nameof(Mediator)}");
        }

        this.handlersFromAssembly = handlersFromAssembly;
    }

    public static QuestyConfigurationBuilder Create(params Assembly[] handlersFromAssembly) => new(handlersFromAssembly);
    
    public QuestyConfigurationBuilder UseMediatorType(Type customMediatorType)
    {
        if (!customMediatorType.IsAssignableTo<IMediator>() || !customMediatorType.IsAssignableTo<ISender>() || !customMediatorType.IsAssignableTo<IPublisher>())
        {
            throw new ArgumentException(
                $"{customMediatorType.Name} needs to be assignable to the following interfaces {nameof(IMediator)}, {nameof(ISender)}, {nameof(IPublisher)}!",
                nameof(customMediatorType));
        }

        this.mediatorType = customMediatorType;
        return this;
    }

    public QuestyConfigurationBuilder UseNotificationPublisher(Type customNotificationPublisherType)
    {
        if (!customNotificationPublisherType.IsAssignableTo<INotificationPublisher>())
        {
            throw new ArgumentException(
                $"{customNotificationPublisherType.Name} is not assignable to type {nameof(INotificationPublisher)}!",
                nameof(customNotificationPublisherType));
        }

        this.notificationPublisherType = customNotificationPublisherType;
        return this;
    }

    public QuestyConfigurationBuilder WithCustomPipelineBehavior(Type customPipelineBehaviorType)
    {
        if (customPipelineBehaviorType is null)
        {
            throw new ArgumentNullException(nameof(customPipelineBehaviorType));
        }

        this.internalCustomPipelineBehaviorTypes.Add(customPipelineBehaviorType);

        return this;
    }

    public QuestyConfigurationBuilder WithCustomPipelineBehaviors(IEnumerable<Type> customPipelineBehaviorTypes)
    {
        if (customPipelineBehaviorTypes is null)
        {
            throw new ArgumentNullException(nameof(customPipelineBehaviorTypes));
        }
        
        foreach (var customPipelineBehaviorType in customPipelineBehaviorTypes)
        {
            this.WithCustomPipelineBehavior(customPipelineBehaviorType);
        }

        return this;
    }

    public QuestyConfigurationBuilder WithCustomStreamPipelineBehavior(Type customStreamPipelineBehaviorType)
    {
        if (customStreamPipelineBehaviorType is null)
        {
            throw new ArgumentNullException(nameof(customStreamPipelineBehaviorType));
        }

        this.internalCustomStreamPipelineBehaviorTypes.Add(customStreamPipelineBehaviorType);

        return this;
    }

    public QuestyConfigurationBuilder WithAllOpenGenericHandlerTypesRegistered()
    {
        foreach (var openGenericHandlerType in KnownHandlerTypes.AllTypes)
        {
            this.AddOpenGenericHandlerToRegister(openGenericHandlerType);
        }

        return this;
    }

    public QuestyConfigurationBuilder WithRegistrationScope(RegistrationScope registrationScope)
    {
        this.registrationScope = registrationScope;
        return this;
    }

    public QuestyConfigurationBuilder WithOpenGenericHandlerTypeToRegister(Type openGenericHandlerType)
    {
        if (!KnownHandlerTypes.AllTypes.Contains(openGenericHandlerType))
        {
            throw new ArgumentException(
                $"Invalid open-generic handler-type {openGenericHandlerType.Name}",
                nameof(openGenericHandlerType));
        }

        this.AddOpenGenericHandlerToRegister(openGenericHandlerType);

        return this;
    }

    public QuestyConfigurationBuilder WithRequestHandlersManuallyRegistered()
    {
        foreach (var openGenericHandlerType in KnownHandlerTypes.AllTypes.Where(type => type != typeof(IRequestHandler<,>)))
        {
            this.AddOpenGenericHandlerToRegister(openGenericHandlerType);
        }

        return this;
    }

    public QuestyConfigurationBuilder WithCustomStreamPipelineBehaviors(IEnumerable<Type> customStreamPipelineBehaviorTypes)
    {
        foreach (var customStreamPipelineBehaviorType in customStreamPipelineBehaviorTypes)
        {
            this.WithCustomStreamPipelineBehavior(customStreamPipelineBehaviorType);
        }

        return this;
    }

    public QuestyConfiguration Build() => new(
            this.handlersFromAssembly,
            this.mediatorType,
            this.notificationPublisherType,
            this.internalOpenGenericHandlerTypesToRegister.ToArray(),
            this.internalCustomPipelineBehaviorTypes.ToArray(),
            this.internalCustomStreamPipelineBehaviorTypes.ToArray(),
            this.registrationScope);
    
    private void AddOpenGenericHandlerToRegister(Type openHandlerType)
    {
        this.internalOpenGenericHandlerTypesToRegister.Add(openHandlerType);
    }
}