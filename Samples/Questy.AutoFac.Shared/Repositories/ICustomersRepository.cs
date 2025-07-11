using System;
using System.Collections.Generic;
using Questy.AutoFac.Shared.Entities;

namespace Questy.AutoFac.Shared.Repositories;

public interface ICustomersRepository
{
    bool AddCustomer(Customer customer);

    ICollection<Customer> GetAll();

    Customer FindCustomer(Guid id);
}