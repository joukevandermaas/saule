using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Saule;

namespace Tests.Models
{
    public class CompanyWithCustomersResource : CompanyResource
    {
        public CompanyWithCustomersResource()
        {
            HasMany<CustomerResource>(nameof(CompanyWithCustomers.Customers));
        }
    }
}
