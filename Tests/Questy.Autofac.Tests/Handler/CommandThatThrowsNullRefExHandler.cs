using System;
using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;

namespace Questy.Autofac.Tests.Handler;

public class CommandThatThrowsNullRefExHandler : IRequestHandler<CommandThatThrowsNullRefException, object>
{
    public Task<object> Handle(CommandThatThrowsNullRefException request, CancellationToken cancellationToken)
    {
        throw new NullReferenceException();
    }
}