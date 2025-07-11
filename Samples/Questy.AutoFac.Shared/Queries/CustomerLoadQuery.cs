using System;
using Questy.AutoFac.Shared.Dto;

namespace Questy.AutoFac.Shared.Queries;

public class CustomerLoadQuery : IRequest<CustomerDto>
{
    public Guid Id { get; }

    public CustomerLoadQuery(Guid id)
    {
        this.Id = id;
    }
}