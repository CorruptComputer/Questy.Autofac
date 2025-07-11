using System.Runtime.CompilerServices;
using Autofac;
using Questy.AutoFac.Extensions;
using Questy.Pipeline;
using Module = Autofac.Module;

namespace Questy.AutoFac;

internal class QuestyModule : Module
{
    private readonly QuestyConfiguration QuestyConfiguration;
    
    private readonly Type[] builtInPipelineBehaviorTypes =
    {
        typeof(RequestPostProcessorBehavior<,>),
        typeof(RequestPreProcessorBehavior<,>),
        typeof(RequestExceptionActionProcessorBehavior<,>),
        typeof(RequestExceptionProcessorBehavior<,>),
    };

    public QuestyModule(QuestyConfiguration QuestyConfiguration)
    {
        this.QuestyConfiguration = QuestyConfiguration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ServiceProviderWrapper>()
            .As<IServiceProvider>()
            .InstancePerDependency()
            .IfNotRegistered(typeof(IServiceProvider));
        
        builder
            .RegisterType(this.QuestyConfiguration.MediatorType)
            .As<IMediator>()
            .As<IPublisher>()
            .As<ISender>()
            .ApplyTargetScope(this.QuestyConfiguration.RegistrationScope);

        builder
            .RegisterType(this.QuestyConfiguration.NotificationPublisherType)
            .As<INotificationPublisher>()
            .ApplyTargetScope(this.QuestyConfiguration.RegistrationScope);

        foreach (var openHandlerType in this.QuestyConfiguration.OpenGenericTypesToRegister)
        {
            builder.RegisterAssemblyTypes(this.QuestyConfiguration.HandlersFromAssemblies)
                .AsClosedTypesOf(openHandlerType)
                .ApplyTargetScope(this.QuestyConfiguration.RegistrationScope);
        }

        foreach (var builtInPipelineBehaviorType in this.builtInPipelineBehaviorTypes)
        {
            this.RegisterGeneric(builder, builtInPipelineBehaviorType, typeof(IPipelineBehavior<,>));
        }
            
        foreach (var customBehaviorType in this.QuestyConfiguration.CustomPipelineBehaviors)
        {
            this.RegisterGeneric(builder, customBehaviorType, typeof(IPipelineBehavior<,>));
        }
        
        foreach (var customBehaviorType in this.QuestyConfiguration.CustomStreamPipelineBehaviors)
        {
            this.RegisterGeneric(builder, customBehaviorType, typeof(IStreamPipelineBehavior<,>));
        }
    }

    private void RegisterGeneric(ContainerBuilder builder, Type implementationType, Type asType)
    {
        builder.RegisterGeneric(implementationType)
            .As(asType)
            .ApplyTargetScope(this.QuestyConfiguration.RegistrationScope);
    }
}