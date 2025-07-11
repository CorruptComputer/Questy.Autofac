using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;

namespace Questy.Autofac.Tests.Handler;

public class CommandHandler : IRequestHandler<VoidCommand>
{
    public Task Handle(VoidCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}