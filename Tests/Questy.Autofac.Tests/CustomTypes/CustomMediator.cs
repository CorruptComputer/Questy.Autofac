using System;

namespace Questy.Autofac.Tests.CustomTypes;

public class CustomMediator : Mediator
{
    public CustomMediator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public CustomMediator(IServiceProvider serviceProvider, INotificationPublisher publisher) : base(serviceProvider, publisher)
    {
    }
}