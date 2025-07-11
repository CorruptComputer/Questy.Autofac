namespace Questy.Autofac;

/// <summary>
///   The scope in which Questy components are registered.
/// </summary>
public enum RegistrationScope
{
    /// <summary>
    ///   Transient registration.
    /// </summary>
    Transient = 0,

    /// <summary>
    ///   Scoped registration.
    /// </summary>
    Scoped = 1,
}