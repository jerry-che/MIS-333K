using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Che_Jerry_HW6.Models
{
    public class Supplier
    {
        public Int32 SupplierID { get; set; }

        [Required(ErrorMessage = "Please include the supplier's name")]
        [Display(Name = "Supplier Name")]
        public String SupplierName { get; set; }

        [Required(ErrorMessage = "Please enter the supplier's email address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Please enter the supplier's phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the established date")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        [Display(Name = "Established Date")]
        [DisplayFormat(DataFormatString ="{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EstablishedDate { get; set; }

        [Display(Name = "Is this a preferred supplier?")]
        public Boolean PreferredStatus { get; set; }

        [Display(Name = "Notes")]
        public String Notes { get; set; }

        public List<SupplierDetail> SupplierDetails { get; set; }

        public Supplier()
        {
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierDetail>();
            }
        }

    }
}