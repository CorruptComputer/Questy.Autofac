using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Shared.Notifications;

namespace Questy.AutoFac.Shared.NotificationHandler;

public class CustomerAddedNotificationHandler : INotificationHandler<CustomerAddedNotification>
{
    public async Task Handle(CustomerAddedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received CustomerAddedNotification for Customer {notification.Name}");

        await Task.CompletedTask.ConfigureAwait(false);
    }
}