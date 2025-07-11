using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;

namespace Questy.AutoFac.Tests.Handler;

public class CommandHandler : IRequestHandler<VoidCommand>
{
    public Task Handle(VoidCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}