using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace scs_Project.Models
{
    public class VendorInfo
    {
        public int id { get; set; }
        [Required]
        public string Vendor_Name { get; set; }
        public string Contact_No { get; set; }
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Dealing_Person { get; set; }
        public Boolean Vendor_Status { get; set; }
        public Boolean Row_Status { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string User_id { get; set; }
        public string Edit_id { get; set; }
    }
    public class VendorInfoVM
    {
        public int id { get; set; }
        public string Vendor_Name { get; set; }
        public string Contact_No { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Dealing_Person { get; set; }
        public Boolean Vendor_Status { get; set; }
        public int MasterID { get; set; }

    }

}