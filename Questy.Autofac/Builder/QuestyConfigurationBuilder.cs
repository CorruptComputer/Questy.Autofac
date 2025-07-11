using System.Reflection;
using Autofac;
using Questy.NotificationPublishers;

namespace Questy.Autofac.Builder;

/// <summary>
///   Builder for configuring Questy settings.
/// </summary>
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

    /// <summary>
    ///   Creates a new instance of <see cref="QuestyConfigurationBuilder"/> with the specified assemblies.
    /// </summary>
    /// <param name="handlersFromAssembly"></param>
    /// <returns></returns>
    public static QuestyConfigurationBuilder Create(params Assembly[] handlersFromAssembly) => new(handlersFromAssembly);
    
    /// <summary>
    ///   Adds an open-generic handler type to the configuration.
    /// </summary>
    /// <param name="customMediatorType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public QuestyConfigurationBuilder UseMediatorType(Type customMediatorType)
    {
        if (!customMediatorType.IsAssignableTo<IMediator>() || !customMediatorType.IsAssignableTo<ISender>() || !customMediatorType.IsAssignableTo<IPublisher>())
        {
            throw new ArgumentException(
                $"{customMediatorType.Name} needs to be assignable to the following interfaces {nameof(IMediator)}, {nameof(ISender)}, {nameof(IPublisher)}!",
                nameof(customMediatorType));
        }

        mediatorType = customMediatorType;
        return this;
    }

    /// <summary>
    ///   Adds a custom notification publisher type to the configuration.
    /// </summary>
    /// <param name="customNotificationPublisherType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public QuestyConfigurationBuilder UseNotificationPublisher(Type customNotificationPublisherType)
    {
        if (!customNotificationPublisherType.IsAssignableTo<INotificationPublisher>())
        {
            throw new ArgumentException(
                $"{customNotificationPublisherType.Name} is not assignable to type {nameof(INotificationPublisher)}!",
                nameof(customNotificationPublisherType));
        }

        notificationPublisherType = customNotificationPublisherType;
        return this;
    }

    /// <summary>
    ///   Adds a custom pipeline behavior type to the configuration.
    /// </summary>
    /// <param name="customPipelineBehaviorType"></param>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithCustomPipelineBehavior(Type customPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(customPipelineBehaviorType);

        internalCustomPipelineBehaviorTypes.Add(customPipelineBehaviorType);

        return this;
    }

    /// <summary>
    ///   Adds multiple custom pipeline behavior types to the configuration.
    /// </summary>
    /// <param name="customPipelineBehaviorTypes"></param>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithCustomPipelineBehaviors(IEnumerable<Type> customPipelineBehaviorTypes)
    {
        ArgumentNullException.ThrowIfNull(customPipelineBehaviorTypes);

        foreach (Type customPipelineBehaviorType in customPipelineBehaviorTypes)
        {
            WithCustomPipelineBehavior(customPipelineBehaviorType);
        }

        return this;
    }

    /// <summary>
    ///   Adds a custom stream pipeline behavior type to the configuration.
    /// </summary>
    /// <param name="customStreamPipelineBehaviorType"></param>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithCustomStreamPipelineBehavior(Type customStreamPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(customStreamPipelineBehaviorType);

        internalCustomStreamPipelineBehaviorTypes.Add(customStreamPipelineBehaviorType);

        return this;
    }

    /// <summary>
    ///   Adds all known generic handler types to the configuration.
    /// </summary>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithAllOpenGenericHandlerTypesRegistered()
    {
        foreach (Type openGenericHandlerType in KnownHandlerTypes.AllTypes)
        {
            AddOpenGenericHandlerToRegister(openGenericHandlerType);
        }

        return this;
    }

    /// <summary>
    ///   Sets the registration scope for the configuration.
    /// </summary>
    /// <param name="registrationScope"></param>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithRegistrationScope(RegistrationScope registrationScope)
    {
        this.registrationScope = registrationScope;
        return this;
    }

    /// <summary>
    ///   Adds an open-generic handler type to the configuration.
    /// </summary>
    /// <param name="openGenericHandlerType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public QuestyConfigurationBuilder WithOpenGenericHandlerTypeToRegister(Type openGenericHandlerType)
    {
        if (!KnownHandlerTypes.AllTypes.Contains(openGenericHandlerType))
        {
            throw new ArgumentException(
                $"Invalid open-generic handler-type {openGenericHandlerType.Name}",
                nameof(openGenericHandlerType));
        }

        AddOpenGenericHandlerToRegister(openGenericHandlerType);

        return this;
    }

    /// <summary>
    ///   Adds all known handler types to the configuration, except for IRequestHandler
    /// </summary>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithRequestHandlersManuallyRegistered()
    {
        foreach (Type? openGenericHandlerType in KnownHandlerTypes.AllTypes.Where(type => type != typeof(IRequestHandler<,>)))
        {
            AddOpenGenericHandlerToRegister(openGenericHandlerType);
        }

        return this;
    }

    /// <summary>
    ///   Adds multiple custom stream pipeline behavior types to the configuration.
    /// </summary>
    /// <param name="customStreamPipelineBehaviorTypes"></param>
    /// <returns></returns>
    public QuestyConfigurationBuilder WithCustomStreamPipelineBehaviors(IEnumerable<Type> customStreamPipelineBehaviorTypes)
    {
        foreach (Type customStreamPipelineBehaviorType in customStreamPipelineBehaviorTypes)
        {
            WithCustomStreamPipelineBehavior(customStreamPipelineBehaviorType);
        }

        return this;
    }

    /// <summary>
    ///   Builds the Questy configuration based on the provided settings.
    /// </summary>
    /// <returns></returns>
    public QuestyConfiguration Build() => new(
            handlersFromAssembly,
            mediatorType,
            notificationPublisherType,
            internalOpenGenericHandlerTypesToRegister.ToArray(),
            internalCustomPipelineBehaviorTypes.ToArray(),
            internalCustomStreamPipelineBehaviorTypes.ToArray(),
            registrationScope);
    
    private void AddOpenGenericHandlerToRegister(Type openHandlerType)
    {
        internalOpenGenericHandlerTypesToRegister.Add(openHandlerType);
    }
}