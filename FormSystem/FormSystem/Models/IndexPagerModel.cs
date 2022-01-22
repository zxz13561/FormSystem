using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormSystem.Models
{
    public class IndexPagerModel
    {
        public string ShowFormsHTML { get; set; }
        public string PagniationHTML { get; set; }
        public int MaxPage { get; set; }
    }
}