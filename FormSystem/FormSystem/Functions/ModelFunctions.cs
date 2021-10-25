using FormSystem.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormSystem.Functions
{
    public class ModelFunctions
    {
        public static List<SelectListItem> QusetionsType()
        {
            // Import Question Type Datas to DropdownList
            List<SelectListItem> QTypeList = new List<SelectListItem>() { };
            var typeQuery = new FormDBModel().QuestionTypes;

            foreach (var type in typeQuery)
            {
                QTypeList.Add(new SelectListItem { Text = type.Name, Value = type.TypeID });
            }

            return QTypeList;
        }
    }
}