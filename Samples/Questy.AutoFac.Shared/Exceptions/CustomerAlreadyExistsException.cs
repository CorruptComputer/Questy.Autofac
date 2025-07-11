using System;

namespace Questy.AutoFac.Shared.Exceptions;

public class CustomerAlreadyExistsException : Exception
{
    public CustomerAlreadyExistsException(string message = "The customer already exists") : base(message)
    {
    }
}