using System.Threading;
using System.Threading.Tasks;
using Questy.AutoFac.Tests.Commands;
using Questy.Pipeline;

namespace Questy.AutoFac.Tests.PreProcessor;

public class PingRequestPreProcessor : IRequestPreProcessor<VoidCommand>
{
    public Task Process(VoidCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}