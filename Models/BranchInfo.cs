using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scs_Project.Models
{
    public class BranchInfo
    {
        public int id { get; set; }
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Focal_Person { get; set; }
        public int DivisionID { get; set; }
        public int TypeID { get; set; }
        public Boolean Row_Status { get; set; }
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
    }

    public class Division
    {
        public int id { get; set; }
        public string DivisionName { get; set; }
    }
    public class BranchType
    {
        public int id { get; set; }
        public string TypeName { get; set; }
    }

}