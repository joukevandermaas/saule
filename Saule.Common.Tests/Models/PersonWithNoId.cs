using System.Collections.Generic;
using Saule.Common.Tests.Helpers;

namespace Saule.Common.Tests.Models
{
    public class PersonWithNoId
    {
        public string FirstName => "John";
        public string LastName => "Smith";
        public int Age => 34;
        public int NumberOfLegs => 2;
        public Company Job => Get.Company();
        public IEnumerable<Person> Friends => new List<Person>();
    }
}