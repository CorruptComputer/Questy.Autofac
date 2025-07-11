using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;
using Questy.Pipeline;

namespace Questy.AutoFac.Tests.ExceptionActions;

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