using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;
using Questy.Pipeline;

namespace Questy.Autofac.Tests.ExceptionActions;

public class CommandThatThrowsNullRefExceptionActionHandler : IRequestExceptionAction<CommandThatThrowsNullRefException,NullReferenceException>
{
    public static int CallCount = 0;
        

    public Task Execute(CommandThatThrowsNullRefException request, NullReferenceException exception,
        CancellationToken cancellationToken)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}