using System;
using System.Collections.Generic;
using Questy.Autofac.Shared.Entities;

namespace Questy.Autofac.Shared.Repositories;

public interface ICustomersRepository
{
    bool AddCustomer(Customer customer);

    ICollection<Customer> GetAll();

    Customer FindCustomer(Guid id);
}