﻿using FormSystem.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormSystem.Functions
{
    public class ModelFunctions
    {
        /// <summary>回傳問題種類下拉選單</summary>
        /// <returns></returns>
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

        /// <summary>依照問題種類回傳HTML code</summary>
        /// <param name="fLayout"></param>
        /// <returns></returns>
        public static string QuestionHTML(FormLayout fLayout)
        {
            string htmlString = string.Empty;

            switch (fLayout.QuestionType)
            {
                case "QT01 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">
                                <input class=""form-control"" type=""text"" name=""{fLayout.Body}"" value="""" />
                            </div>
                        </div>";
                    break;

                case "QT02 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">
                                <input class=""form-control"" type=""number"" name=""{fLayout.Body}"" value="""" />
                            </div>
                        </div>";
                    break;

                case "QT03 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">
                                <input class=""form-control"" type=""email"" name=""{fLayout.Body}"" value="""" />
                            </div>
                        </div>";
                    break;

                case "QT04 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">
                                <input class=""form-control"" type=""date"" name=""{fLayout.Body}"" value="""" />
                            </div>
                        </div>";
                    break;

                case "QT05 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">";

                    foreach(string ans in fLayout.Answer.Split(';'))
                    {
                        htmlString += $@"
                               <div class=""row"">
                                    <div class=""col-1""></div>
                                    <div class=""form-check col-11"">
                                    <input class=""form-check-input col-1"" type=""radio"" value=""{ans}"">
                                    <label class=""form-check-label col-10"">{ans}</label>
                                    </div>
                                </div>";
                    }

                    htmlString +=@"
                            </div>
                        </div>";
                    break;

                case "QT06 ":
                    htmlString = $@"
                        <div class=""mb-3 row"">
                            <label class=""col-sm-2 col-form-label"">{fLayout.Body}</label>                            
                            <div class=""col-sm-6"">";

                    foreach (string ans in fLayout.Answer.Split(';'))
                    {
                        htmlString += $@"
                               <div class=""row"">
                                    <div class=""col-1""></div>
                                    <div class=""form-check col-11"">
                                    <input class=""form-check-input col-1"" type=""checkbox"" value=""{ans}"">
                                    <label class=""form-check-label col-10"">{ans}</label>
                                    </div>
                                </div>";
                    }

                    htmlString += @"
                            </div>
                        </div>";
                    break;

                default:
                    break;
            }

            return htmlString;
        }
    }
}