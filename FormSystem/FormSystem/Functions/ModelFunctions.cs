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
        /// <summary>回傳首頁問題列表的HTML code</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string IndexListHTML(int _Qindex = 1, int _howManyInPage = 20)
        {
            string tableString = string.Empty;
            var _allQList = new FormDBModel().FormInfoes.OrderByDescending(f => f.CreateDate).ToList();

            // Create Table HTML
            for (int i = _Qindex - 1; (i < _Qindex + _howManyInPage - 1) && (i < _allQList.Count); i++)
            {
                var data = _allQList[i];

                string status = DateTime.Now > data.StartDate  && DateTime.Now < data.EndDate ? "投票中" : "關閉中";
                tableString += $@"
                    <tr>
                        <th scope=""row"">{i + 1}</th>
                        <td><a href=""/Home/FillForm/{data.FormID}"">{data.Name}</a></td>
                        <td>{status}</td>
                        <td>{data.StartDate.ToString("yyyy-MM-dd")}</td>
                        <td>{data.EndDate.ToString("yyyy-MM-dd")}</td>
                        <td><a href=""Home/FrontAnalysis?FormID={data.FormID}"">前往</a></td>
                    </tr>";
            }

            return tableString;
        }

        /// <summary>回傳列表的分頁HTML code</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string PaginationHTML(int _totalListCount, int _howManyInPage = 2)
        {
            string pagehtml = string.Empty;
            int _pageTail = _totalListCount % _howManyInPage == 0 ? _totalListCount / _howManyInPage : _totalListCount / _howManyInPage + 1;

            for (int i = 0; i < _pageTail; i++)
            {
                pagehtml += $"<li class=\"page-item\"><a class=\"page-link\" href=\"#\" onclick=\"hideByPager({i + 1})\">{i + 1}</a></li>";
            }

            return pagehtml;
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
                string isNeed = data.NeedAns ? "是" : "否";

                tableString += $@"
                    <tr>
                        <td>
                            <input type=""checkbox"" id=""{data.Body}"" name=""chkLayout[{i}]"" value=""{data.Body}"">
                            <input name=""chkLayout[{i}]"" type=""hidden"" value=""false"">
                        </td>
                        <td>{data.QuestionSort}</td>
                        <td>{data.Body}</td>
                        <td>{DALFunctions.GetQuestionTypeName(data.QuestionType)}</td>
                        <td>{isNeed}</td>
                        <td><button type=""button"" class=""btn btn-secondary btn-sm"" onclick=""EditLayoutAjax({i})"">編輯</button></td>
                      </tr>";
                i++;
            }

            return tableString;
        }
    }
}