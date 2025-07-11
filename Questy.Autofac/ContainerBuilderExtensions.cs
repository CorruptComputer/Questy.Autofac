using Autofac;

namespace Questy.Autofac;

/// <summary>
///   Provides extension methods for registering Questy in an Autofac container.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    ///   Registers Questy in the provided Autofac container builder with the specified configuration.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ContainerBuilder RegisterQuesty(
        this ContainerBuilder builder,
        QuestyConfiguration configuration)
    {
        builder.RegisterModule(new QuestyModule(configuration));

        return builder;
    }
}