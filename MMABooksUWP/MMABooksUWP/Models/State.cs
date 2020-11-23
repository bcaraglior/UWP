using System;
using System.Collections.Generic;

namespace MMABooksUWP.Models
{
    public partial class State
    {
        public State()
        {
            Customers = new HashSet<Customer>();
        }

        public string StateCode { get; set; }
        public string StateName { get; set; }

        public override string ToString()
        {
            return StateCode + ", " + StateName;
        }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
