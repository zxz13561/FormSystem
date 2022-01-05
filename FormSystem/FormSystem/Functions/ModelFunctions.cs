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

        /// <summary>回傳問題列表的HTML code</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string LayoutListHTML(List<FormLayout> list)
        {
            int i = 0;
            string tableString = string.Empty;

            // Create Table HTML
            foreach (var data in list)
            {
                tableString += $@"
                    <tr>
                        <td>
                            <input type=""checkbox"" id=""{data.Body}"" name=""chkLayout[{i}]"" value=""{data.Body}"">
                            <input name=""chkLayout[{i}]"" type=""hidden"" value=""false"">
                        </td>
                        <td>{data.QuestionSort}</td>
                        <td>{data.Body}</td>
                        <td>{DALFunctions.GetQuestionTypeNme(data.QuestionType)}</td>
                        <td><a href=""Home/Index"">編輯</a></td>
                    </tr>";
                i++;
            }

            return tableString;
        }
    }
}