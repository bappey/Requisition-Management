using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scs_Project.Models
{
    public class DepartmentInfo
    {
        public int id { get; set; }
        public int BranchId { get; set; }
        public string Focal_Person { get; set; }

        public int DepartmentId { get; set; }
        public string Contact { get; set; }
        public Boolean Row_Status { get; set; }
        public string Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public string Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }

    }
    public class DeptListViewModel
    {
        public DepartmentInfo DeptInfo { get; set; }
        public BranchInfo BInfo { get; set; }
        public DepartmentEntry D_Entry { get; set; }
    }

    public class DepartmentEntry
    {
        public int id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Boolean Row_Status { get; set; }
        public string Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public string Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
    }
}