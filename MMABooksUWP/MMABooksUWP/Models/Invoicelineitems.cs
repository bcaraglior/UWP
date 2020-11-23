using System;
using System.Collections.Generic;

namespace MMABooksUWP.Models
{
    public partial class Invoicelineitems
    {
        public int InvoiceId { get; set; }
        public string ProductCode { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }

        public virtual Invoices Invoice { get; set; }
        public virtual Products ProductCodeNavigation { get; set; }
    }
}
