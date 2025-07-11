using System;
using Questy.Autofac.Shared.Dto;

namespace Questy.Autofac.Shared.Queries;

public class CustomerLoadQuery : IRequest<CustomerDto>
{
    public Guid Id { get; }

    public CustomerLoadQuery(Guid id)
    {
        this.Id = id;
    }
}