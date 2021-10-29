using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormSystem.DBModel;
using FormSystem.Models;
using FormSystem.Functions;

namespace FormSystem.Controllers
{
    public class HomeController : Controller
    {
        #region Only Show Page Controllers
        public ActionResult Index()
        {
            var InfoList = new FormDBModel().FormInfoes.ToList();
            return View(InfoList);
        }

        public ActionResult Login()
        {
            ViewBag.Message = "請輸入管理員帳號密碼";

            return View();
        }

        public ActionResult FormManager()
        {
            var InfoList = new FormDBModel().FormInfoes.ToList();
            return View(InfoList);
        }

        /// <summary>顯示常用問題頁面</summary>
        /// <returns></returns>
        public ActionResult FrequentlyQuestions()
        {
            // Set Drop Down List
            ViewBag.SelectList = ModelFunctions.QusetionsType();
            // Save data into viewbag
            ViewBag.FrequenList = DALFunctions.FrequentlyQuestionsList();

            return View(new FrenquenQuestion()); ;
        }

        public ActionResult ErrorPage(string errMsg)
        {
            ViewBag.ErrMsg = errMsg;
            return View();
        }
        #endregion

        #region Show Form Layout and Receive Answer
        /// <summary>從DB抓取資料顯示表單</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FillForm(string id)
        {
            try
            {
                // check id is correct
                ViewBag.Message = id;
                Session["FillFormFID"] = id;
                if (!Guid.TryParse(id, out Guid FID))
                {
                    ViewBag.FormTitle = "發生錯誤";
                    ViewBag.FormBody = "請回上一頁重新選取";
                    ViewBag.FormStart = "";
                    ViewBag.FormEnd = "";

                    return View();
                }

                // Show Form Layout
                using (FormDBModel dbModel = new FormDBModel())
                {
                    // Get Form information
                    var formInfo = dbModel.FormInfoes
                            .Where(info => info.FormID == FID)
                            .FirstOrDefault();

                    ViewBag.FormTitle = formInfo.Name;
                    ViewBag.FormBody = formInfo.Body;
                    ViewBag.FormStart = formInfo.StartDate.ToString("yyyy-MM-dd hh:mm:ss");
                    ViewBag.FormEnd = formInfo.EndDate.ToString("yyyy-MM-dd hh:mm:ss");

                    // Get Form layout and sent to view page
                    var formLayout = new FormDBModel().FormLayouts
                            .Where(layout => layout.FormID == FID)
                            .OrderBy(layout => layout.ID);

                    // Save into Session
                    Session["FillFormInfo"] = formLayout.ToList();
                    return View(formLayout.ToList());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>接收表單資料，顯示確認頁面</summary>
        /// <param name="ansList"></param>
        /// <returns></returns>
        public ActionResult CheckAns(List<FormAnsModel> ansList)
        {
            try
            {
                // Get FID and Info from Session
                if (!Guid.TryParse(Session["FillFormFID"].ToString(), out Guid fillFid) || Session["FillFormInfo"] == null)
                {
                    throw new Exception("FillFormInfo Null");
                }

                // Show Answers
                List<FormLayout> fLayout = (List<FormLayout>)Session["FillFormInfo"];
                List<ShowAnsModel> showAns = new List<ShowAnsModel>();
                FormData sessionAns = new FormData() { FormID = fillFid};

                for (int i = 0; i < fLayout.Count; i++)
                {
                    sessionAns.QuestionSort += fLayout[i].QuestionType + ";";
                    sessionAns.AnswerData += (ansList[i].Answer.Count() > 1) ? string.Join(",", ansList[i].Answer) : ansList[i].Answer[0];
                    sessionAns.AnswerData += ";";
                    string ansStr = (ansList[i].Answer.Count() > 1) ? string.Join(",", ansList[i].Answer.Where((source, value) => source != "false").ToArray()) : ansList[i].Answer[0];
                    showAns.Add(new ShowAnsModel() { QBody = fLayout[i].Body, QAns = ansStr });
                }

                // Save to session and output
                Session["FormData"] = sessionAns;
                return View(showAns);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>從Session將表單答案寫入DB</summary>
        /// <returns></returns>
        public ActionResult WriteAnsToDB()
        {
            try
            {
                // Get data from Session
                if (Session["FormData"] == null)
                {
                    throw new Exception("Session Data is null");
                }

                FormData fData =  (FormData)Session["FormData"];
                fData.CreateDate = DateTime.Now;

                using (FormDBModel db = new FormDBModel())
                {
                    db.FormDatas.Add(fData);
                    db.SaveChanges();
                }

                return View("SuccessPage");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }
        #endregion

        #region Form Manager Function Controller
        /// <summary>依照參數選擇是新增表單或者編輯表單</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditForm(string id)
        {
            // Set Drop Down List
            ViewBag.SelectList = ModelFunctions.QusetionsType();

            // Create New Form
            if (id == "NewForm")
            {
                CreateFormModel cModel = new CreateFormModel();
                cModel.mInfo = new FormInfo();
                cModel.mLayout = new FormLayout();
                Guid newID = Guid.NewGuid();
                cModel.mInfo.FormID = newID;
                Session["FID"] = newID;
                Session["LayoutList"] = null;
                Session["FormInfo"] = null;

                return View("CreateNewForm", cModel);
            }

            return View();
        }

        /// <summary>轉換Session資料，存入SQL</summary>
        /// <returns></returns>
        public ActionResult InsertIntoDB()
        {
            FormInfo fInfo = (Session["FormInfo"] != null) ? (FormInfo)Session["FormInfo"] : null;
            List<FormLayout> fLayout = (Session["LayoutList"] != null) ? (List<FormLayout>)Session["LayoutList"] : null;

            try
            {
                if (fInfo != null && fLayout != null)
                {
                    using (FormDBModel db = new FormDBModel())
                    {
                        // Save Info to DB
                        fInfo.CreateDate = DateTime.Now;
                        db.FormInfoes.Add(fInfo);
                        db.SaveChanges();

                        // Save Layout to DB
                        foreach (FormLayout data in fLayout)
                        {
                            db.FormLayouts.Add(data);
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("DB Error");
                }

                return RedirectToAction("FormManager", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>從DB刪除所選的表單與相關的資料</summary>
        /// <param name="chkForm"></param>
        /// <returns></returns>
        public ActionResult DeleteForm(string[] chkForm)
        {
            try
            {
                using (FormDBModel db = new FormDBModel())
                {
                    foreach (string _fid in chkForm)
                    {
                        if(_fid != "false"){
                            // Check FID is correct
                            if (!Guid.TryParse(_fid, out Guid _FormID))
                                throw new Exception("FID Error");

                            // Delete Children Data First
                            var deleteData =
                            $@"
                                DELETE FROM [dbo].[FormData]
                                WHERE [FormID] = '{_FormID}'
                            ";
                            db.Database.ExecuteSqlCommand(deleteData);

                            var deleteLayout =
                            $@"
                                DELETE FROM [dbo].[FormLayout]
                                WHERE [FormID] = '{_FormID}'
                            ";
                            db.Database.ExecuteSqlCommand(deleteLayout);

                            // Delete Parent Data in the end
                            var deleteInfo =
                            $@"
                                DELETE FROM [dbo].[FormInfo]
                                WHERE [FormID] = '{_FormID}'
                            ";
                            db.Database.ExecuteSqlCommand(deleteInfo);
                        }
                    }
                }
                return RedirectToAction("FormManager");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>移除所選問題，並將新表單存入Session</summary>
        /// <param name="chkLayout"></param>
        /// <returns></returns>
        public ActionResult DeleteLayout(string[] chkLayout)
        {
            try
            {
                // Get Data from session
                List<FormLayout> oldList = (Session["LayoutList"] != null) ? (List<FormLayout>)Session["LayoutList"] : new List<FormLayout>();
                List<FormLayout> newList = new List<FormLayout>();
                int newSort = 0;

                // create new layout list which layout is not checked
                for(int index = 0; index < chkLayout.Count(); index++)
                {
                    if(chkLayout[index] == "false")
                    {
                        newList.Add(oldList[index]);
                        newList[newSort].QuestionSort = newSort + 1;
                        newSort++;
                    }
                }

                // Create Table HTML
                int i = 0;
                string tableString = string.Empty;

                foreach (var data in newList)
                {
                    tableString += $@"
                    <tr>
                        <td>
                            <input type=""checkbox"" id=""{data.Body}"" name=""chkLayout[{i}]"" value=""{data.Body}"">
                            <input name=""chkLayout[{i}]"" type=""hidden"" value=""false"">
                        </td>
                        <td>{data.QuestionSort}</td>
                        <td>{data.Body}</td>
                        <td>{data.QuestionType}</td>
                        <td><a href=""Home/Index"">編輯</a></td>
                    </tr>";
                    i++;
                }

                Session["LayoutList"] = newList;
                return Content(tableString);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }
        #endregion

        #region Create New Form
        /// <summary>新表單資訊</summary>
        /// <param name="fInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewInfo(FormInfo fInfo)
        {
            // Save into Session
            Session["FormInfo"] = fInfo;

            return Content("資料儲存成功");
        }

        /// <summary>新表單問題列表</summary>
        /// <param name="fLay"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewLayout(FormLayout fLay)
        {
            // Check session data exist
            List<FormLayout> list = (Session["LayoutList"] != null) ? (List<FormLayout>)Session["LayoutList"] : new List<FormLayout>();
            Guid.TryParse(Session["FID"].ToString(), out Guid fid);

            FormLayout questionInfo = new FormLayout() {
                FormID = fid,
                Body = fLay.Body,
                Answer = fLay.Answer,
                QuestionType = fLay.QuestionType,
                NeedAns = fLay.NeedAns,
                QuestionSort = (Session["LayoutList"] == null) ? 1 : list.Count() + 1
            };

            list.Add(questionInfo);
            Session["LayoutList"] = list;

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
                        <td>{data.QuestionType}</td>
                        <td><a href=""Home/Index"">編輯</a></td>
                    </tr>";
                i++;
            }

            return Content(tableString);
        }

        /// <summary>顯示新表單</summary>
        /// <returns></returns>
        public ActionResult ShowForm()
        {
            string FormHtml = string.Empty;
            FormInfo fInfo = (Session["FormInfo"] != null ) ? (FormInfo)Session["FormInfo"] : new FormInfo();
            List<FormLayout> fLayout = (Session["LayoutList"] != null) ? (List<FormLayout>)Session["LayoutList"] : new List<FormLayout>();

            FormHtml += $@"
                <div class=""jumbotron"">
                    <div class=""row"">
                        <div class=""col-7"">
                            <h1>{fInfo.Name}</h1>
                        </div>
                        <div class=""col-5"">
                            <h6>開放時間 : {fInfo.StartDate} ~ {fInfo.EndDate}</h6>
                        </div>
                    </div>
                    <p class=""lead"">{fInfo.Body}</p>
               </div>
               <hr />";

            foreach(var layout in fLayout)
            {
                FormHtml += ModelFunctions.QuestionHTML(layout);
            }

            return Content(FormHtml);
        }
        #endregion

        #region Frequenly Question Controller
        /// <summary>新常用問題資料寫入DB</summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult AddFrequentQuestion(FrenquenQuestion q)
        {
            try
            {
                // Add into DB
                using (FormDBModel db = new FormDBModel())
                {
                    db.FrenquenQuestions.Add(q);
                    db.SaveChanges();
                }

                // Set Page need data
                ViewBag.SelectList = ModelFunctions.QusetionsType();
                ViewBag.FrequenList = DALFunctions.FrequentlyQuestionsList();

                return View("FrequentlyQuestions", new FrenquenQuestion());
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>刪除常用問題</summary>
        /// <param name="chkfreq"></param>
        /// <returns></returns>
        public ActionResult DeleteFrequentQuestions(string[] chkfreq)
        {
            try
            {
                using (FormDBModel db = new FormDBModel())
                {
                    foreach(string chk in chkfreq)
                    {
                        if(chk != "false")
                        {
                            var deleteData =
                            $@"
                                DELETE FROM [dbo].[FrenquenQuestion]
                                WHERE [ID] = '{chk}'
                             ";
                            db.Database.ExecuteSqlCommand(deleteData);
                        }
                    }
                }
                // Set Page need data
                ViewBag.SelectList = ModelFunctions.QusetionsType();
                ViewBag.FrequenList = DALFunctions.FrequentlyQuestionsList();

                return View("FrequentlyQuestions", new FrenquenQuestion());
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }
        #endregion
    }
}