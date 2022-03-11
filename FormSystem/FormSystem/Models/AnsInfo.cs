using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormSystem.Models
{
    public class AnsInfo
    {
        public Guid FormID { get; set; }
        public int DataID { get; set; }
        public string AnsHead { get; set; }
        public string CreateDate { get; set; }
    }
}