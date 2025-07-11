using Autofac;
using Questy.Autofac.Extensions;
using Questy.Pipeline;
using Module = Autofac.Module;

namespace Questy.Autofac;

internal class QuestyModule(QuestyConfiguration QuestyConfiguration) : Module
{    
    private readonly Type[] builtInPipelineBehaviorTypes =
    {
        typeof(RequestPostProcessorBehavior<,>),
        typeof(RequestPreProcessorBehavior<,>),
        typeof(RequestExceptionActionProcessorBehavior<,>),
        typeof(RequestExceptionProcessorBehavior<,>),
    };

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ServiceProviderWrapper>()
            .As<IServiceProvider>()
            .InstancePerDependency()
            .IfNotRegistered(typeof(IServiceProvider));
        
        builder
            .RegisterType(QuestyConfiguration.MediatorType)
            .As<IMediator>()
            .As<IPublisher>()
            .As<ISender>()
            .ApplyTargetScope(QuestyConfiguration.RegistrationScope);

        builder
            .RegisterType(QuestyConfiguration.NotificationPublisherType)
            .As<INotificationPublisher>()
            .ApplyTargetScope(QuestyConfiguration.RegistrationScope);

        foreach (Type openHandlerType in QuestyConfiguration.OpenGenericTypesToRegister)
        {
            builder.RegisterAssemblyTypes(QuestyConfiguration.HandlersFromAssemblies)
                .AsClosedTypesOf(openHandlerType)
                .ApplyTargetScope(QuestyConfiguration.RegistrationScope);
        }

        foreach (Type builtInPipelineBehaviorType in builtInPipelineBehaviorTypes)
        {
            RegisterGeneric(builder, builtInPipelineBehaviorType, typeof(IPipelineBehavior<,>));
        }
            
        foreach (Type customBehaviorType in QuestyConfiguration.CustomPipelineBehaviors)
        {
            RegisterGeneric(builder, customBehaviorType, typeof(IPipelineBehavior<,>));
        }
        
        foreach (Type customBehaviorType in QuestyConfiguration.CustomStreamPipelineBehaviors)
        {
            RegisterGeneric(builder, customBehaviorType, typeof(IStreamPipelineBehavior<,>));
        }
    }

    private void RegisterGeneric(ContainerBuilder builder, Type implementationType, Type asType)
    {
        builder.RegisterGeneric(implementationType)
            .As(asType)
            .ApplyTargetScope(QuestyConfiguration.RegistrationScope);
    }
}