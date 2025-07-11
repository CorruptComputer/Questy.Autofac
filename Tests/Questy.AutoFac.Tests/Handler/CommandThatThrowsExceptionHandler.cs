using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;

namespace Questy.AutoFac.Tests.Handler;

public class CommandThatThrowsExceptionHandler : IRequestHandler<CommandThatThrowsArgumentException, object>
{
    public Task<object> Handle(CommandThatThrowsArgumentException request, CancellationToken cancellationToken)
    {
        throw new ArgumentException();
    }
}