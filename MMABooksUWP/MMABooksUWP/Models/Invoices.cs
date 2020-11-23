using System;
using System.Collections.Generic;

namespace MMABooksUWP.Models
{
    public partial class Invoices
    {
        public Invoices()
        {
            Invoicelineitems = new HashSet<Invoicelineitems>();
        }

        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal ProductTotal { get; set; }
        public decimal SalesTax { get; set; }
        public decimal Shipping { get; set; }
        public decimal InvoiceTotal { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Invoicelineitems> Invoicelineitems { get; set; }
    }
}
