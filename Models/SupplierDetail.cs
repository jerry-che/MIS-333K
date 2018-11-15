using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Che_Jerry_HW6.Models
{
    public class SupplierDetail
    {
        public Int32 SupplierDetailID { get; set; }

        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
    }
}
