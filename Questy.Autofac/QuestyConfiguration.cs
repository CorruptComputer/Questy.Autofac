using System.Reflection;

namespace Questy.Autofac;

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
        this.HandlersFromAssemblies = fromAssemblies ?? throw new ArgumentNullException(nameof(fromAssemblies));
        this.MediatorType = mediatorType ?? throw new ArgumentNullException(nameof(mediatorType));
        this.NotificationPublisherType = notificationPublisherType ?? throw new ArgumentNullException(nameof(notificationPublisherType));
        this.OpenGenericTypesToRegister = openGenericTypesToRegister ?? throw new ArgumentNullException(nameof(openGenericTypesToRegister));
        this.CustomPipelineBehaviors = customPipelineBehaviors ?? Array.Empty<Type>();
        this.CustomStreamPipelineBehaviors = customStreamPipelineBehaviors ?? Array.Empty<Type>();
        this.RegistrationScope = registrationScope;
    }
}