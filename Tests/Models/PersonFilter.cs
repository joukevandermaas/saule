using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class PersonFilter
    {
        public bool HideLastName { get; set; }
        public int? MinAge { get; set; }
    }
}
