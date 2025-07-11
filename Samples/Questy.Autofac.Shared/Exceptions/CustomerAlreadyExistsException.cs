using System;

namespace Questy.Autofac.Shared.Exceptions;

public class CustomerAlreadyExistsException : Exception
{
    public CustomerAlreadyExistsException(string message = "The customer already exists") : base(message)
    {
    }
}