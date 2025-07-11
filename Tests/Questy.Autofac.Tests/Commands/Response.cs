using System;

namespace Questy.Autofac.Tests.Commands;

public class Response
{
    public DateTime PingReceivedAt { get; }

    public Response(DateTime pingReceivedAt)
    {
        this.PingReceivedAt = pingReceivedAt;
    }
}