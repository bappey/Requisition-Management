using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCS_Inventory.Models
{
    public class MailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int rId { get; set; }
    }
}