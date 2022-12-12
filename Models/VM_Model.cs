using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCS_Inventory.Models
{
    public class VM_Model
    {
    }
    public class RequsitionStatus
    {
        public int MasterId { get; set; }
        public DateTime Requisition_Date { get; set; }
        public string Requsition_Type { get; set; }
        public string BranchName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

    }
}