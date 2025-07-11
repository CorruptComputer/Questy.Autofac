using System.Reflection;

namespace Questy.Autofac;

/// <summary>
///   Represents the configuration for Questy.
/// </summary>
public class QuestyConfiguration
{
    internal Assembly[] HandlersFromAssemblies { get; }

    internal Type MediatorType { get; }

    internal Type NotificationPublisherType { get; }

    internal Type[] CustomPipelineBehaviors { get; }

    internal Type[] CustomStreamPipelineBehaviors { get; }

    internal Type[] OpenGenericTypesToRegister { get; }

    internal RegistrationScope RegistrationScope { get; }

    internal QuestyConfiguration(
        Assembly[] fromAssemblies,
        Type mediatorType,
        Type notificationPublisherType,
        Type[] openGenericTypesToRegister,
        Type[]? customPipelineBehaviors = null,
        Type[]? customStreamPipelineBehaviors = null,
        RegistrationScope registrationScope = RegistrationScope.Transient)
    {
        HandlersFromAssemblies = fromAssemblies ?? throw new ArgumentNullException(nameof(fromAssemblies));
        MediatorType = mediatorType ?? throw new ArgumentNullException(nameof(mediatorType));
        NotificationPublisherType = notificationPublisherType ?? throw new ArgumentNullException(nameof(notificationPublisherType));
        OpenGenericTypesToRegister = openGenericTypesToRegister ?? throw new ArgumentNullException(nameof(openGenericTypesToRegister));
        CustomPipelineBehaviors = customPipelineBehaviors ?? [];
        CustomStreamPipelineBehaviors = customStreamPipelineBehaviors ?? [];
        RegistrationScope = registrationScope;
    }
}