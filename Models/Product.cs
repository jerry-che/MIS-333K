using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Che_Jerry_HW6.Models
{
    public class Product
    {
        public Int32 ProductID { get; set; }

        [Display(Name = "SKU")]
        public Int32 SKU { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessage = "Product name is required")]
        public String ProductName { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "The value is invalid")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public Decimal ProductPrice { get; set; }

        [Display(Name = "Description")]
        public String Description { get; set; }

        public List<OrderDetail>  OrderDetails { get; set; }
        public List<SupplierDetail> SupplierDetails { get; set; }

        public Product()
        {
            if(OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }

            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierDetail>();
            }
        }
    }




    
}
