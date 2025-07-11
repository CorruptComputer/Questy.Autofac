using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;

namespace Questy.AutoFac.Tests.Handler;

public class UnconstraintedCommandHandler : IRequestHandler<UnconstraintedCommand, int>
{
    public Task<int> Handle(UnconstraintedCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}