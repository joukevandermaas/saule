using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class LazyPerson
    {
        public virtual string Identifier { get; set; }
        public virtual int NumberOfLegs { get; set; }
    }
}
