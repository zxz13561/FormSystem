using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormSystem.DBModel;

namespace FormSystem.Models
{
    public class CreateFormModel
    {
        public FormInfo mInfo { get; set; }

        public FormLayout mLayout { get; set; }
    }
}