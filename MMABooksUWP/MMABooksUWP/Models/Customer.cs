using System;
using System.Collections.Generic;

namespace MMABooksUWP.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Invoices = new HashSet<Invoices>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }

        public override string ToString()
        {
            return CustomerId + ", " + Name + ", " + Address + ", " + City + ", " + State + ", " + ZipCode;
        }

        public virtual State State { get; set; }
        public virtual ICollection<Invoices> Invoices { get; set; }
    }
}
