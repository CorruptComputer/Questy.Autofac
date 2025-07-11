using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;
using Questy.Pipeline;

namespace Questy.Autofac.Tests.ExceptionHandler;

public class NonSpecificExceptionHandler : IRequestExceptionHandler<CommandThatThrowsArgumentException, object, Exception>
{
    public static int CallCount = 0;
    public static DateTime CallTime;
        
    public Task Handle(
        CommandThatThrowsArgumentException request,
        Exception exception,
        RequestExceptionHandlerState<object> state,
        CancellationToken cancellationToken)
    {
        CallTime = DateTime.UtcNow;
        CallCount++;
        state.SetHandled(new object());
        return Task.CompletedTask;
    }
}