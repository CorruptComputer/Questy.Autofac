using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;

namespace Questy.AutoFac.Tests.Handler;

public class CommandThatThrowsNullRefExHandler : IRequestHandler<CommandThatThrowsNullRefException, object>
{
    public Task<object> Handle(CommandThatThrowsNullRefException request, CancellationToken cancellationToken)
    {
        throw new NullReferenceException();
    }
}