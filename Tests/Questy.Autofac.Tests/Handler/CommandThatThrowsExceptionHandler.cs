using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;

namespace Questy.Autofac.Tests.Handler;

public class CommandThatThrowsExceptionHandler : IRequestHandler<CommandThatThrowsArgumentException, object>
{
    public Task<object> Handle(CommandThatThrowsArgumentException request, CancellationToken cancellationToken)
    {
        throw new ArgumentException();
    }
}