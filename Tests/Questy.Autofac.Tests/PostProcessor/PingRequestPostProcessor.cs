using System.Threading;
using System.Threading.Tasks;
using Questy.Autofac.Tests.Commands;
using Questy.Pipeline;

namespace Questy.Autofac.Tests.PostProcessor;

public class PingRequestPostProcessor : IRequestPostProcessor<VoidCommand, Unit>
{
    public Task Process(VoidCommand request, Unit response, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}