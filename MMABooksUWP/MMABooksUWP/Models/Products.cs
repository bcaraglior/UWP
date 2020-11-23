using System;
using System.Collections.Generic;

namespace MMABooksUWP.Models
{
    public partial class Products
    {
        public Products()
        {
            Invoicelineitems = new HashSet<Invoicelineitems>();
        }

        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int OnHandQuantity { get; set; }

        public virtual ICollection<Invoicelineitems> Invoicelineitems { get; set; }
    }
}
