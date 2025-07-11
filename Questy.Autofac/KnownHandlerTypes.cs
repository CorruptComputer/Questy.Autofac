using Questy.Pipeline;

namespace Questy.Autofac;

/// <summary>
///   Contains all known handler types used in Questy.
/// </summary>
public static class KnownHandlerTypes
{
    /// <summary>
    ///   All known handler types that Questy uses.
    /// </summary>
    public static readonly Type[] AllTypes =
    [
        typeof(IRequestPreProcessor<>),
        typeof(IRequestHandler<,>),
        typeof(IRequestHandler<>),
        typeof(IStreamRequestHandler<,>),
        typeof(IRequestPostProcessor<,>),
        typeof(IRequestExceptionHandler<,,>),
        typeof(IRequestExceptionAction<,>),
        typeof(INotificationHandler<>),
    ];
}