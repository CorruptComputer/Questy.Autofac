using Autofac;

namespace Questy.Autofac;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder RegisterQuesty(
        this ContainerBuilder builder,
        QuestyConfiguration configuration)
    {
        builder.RegisterModule(new QuestyModule(configuration));

        return builder;
    }
}