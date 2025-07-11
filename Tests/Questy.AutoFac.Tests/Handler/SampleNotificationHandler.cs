using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;

namespace Questy.AutoFac.Tests.Handler;

public class SampleNotificationHandler : INotificationHandler<SampleNotification>
{
    public async Task Handle(SampleNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received notification: {notification.Message}");
        await Task.CompletedTask;
    }
}