using System.Collections.Generic;

namespace Saule.Common.Tests.Models
{
    public class CompanyWithCustomers : Company
    {
        public CompanyWithCustomers(bool prefill = false, string id = "456")
            : base (prefill, id)
        {
        }

        public IEnumerable<Customer> Customers { get; set; }
    }
}
