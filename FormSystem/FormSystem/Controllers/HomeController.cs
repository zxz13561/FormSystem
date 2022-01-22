using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormSystem.DBModel;
using FormSystem.Models;
using FormSystem.Functions;
using Newtonsoft.Json;

namespace FormSystem.Controllers
{
    public class HomeController : Controller
    {
        #region Show Page Controllers
        public ActionResult Index(int pagination = 0)
        {
            if(pagination != 0)
            {
                return View();
            }
            return View();
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
            // Reset Session
            Session["FrequentIndex"] = null;

            // Set Drop Down List
            ViewBag.SelectList = DALFunctions.QusetionsType();

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

        #region Index Ajax Controllers
        /// <summary>首頁問題列表</summary>
        /// <param name="fLay"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowQuestionsTable()
        {
            // send html code
            IndexPageInfo indexHTML = new IndexPageInfo()
            {
                ShowFormsHTML =  ModelFunctions.IndexListHTML(),  // questions list
                PagniationHTML = ModelFunctions.PaginationHTML() // pagination list
            };

            return Json(indexHTML, JsonRequestBehavior.AllowGet);
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
            ViewBag.SelectList = DALFunctions.QusetionsType();
            ViewBag.FrequenQList = DALFunctions.frequenQList();

            // Create New Form
            if (id == "NewForm")
            {
                // New FID
                Guid newID = Guid.NewGuid();

                // Reset Session
                Session["FID"] = newID;
                Session["LayoutList"] = null;
                Session["FormInfo"] = null;

                // Set View Data
                ViewData["InfoData"] = new FormInfo() { FormID = newID};
                ViewData["LayoutData"] = new FormLayout() { FormID = newID};

                return View("CreateNewForm");
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

        /// <summary>選擇常用問題選項，將資料顯示在輸入格內</summary>
        /// <param name="selectQ"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectFreqQAndReturnDBData(FormCollection selectQ)
        {
            // parse question ID to int
            int selectedID = int.Parse(selectQ[0]);

            // set DB data into frequent question model
            FrenquenQuestion DBfreqQ = DALFunctions.GetFreQInfo(selectedID);
            FormLayout freqQInfo = new FormLayout()
            {
                FormID = new Guid(),// this is not important data; therefor use new guid
                QuestionType = DBfreqQ.QuestionType,
                Body = DBfreqQ.Body,
                Answer = DBfreqQ.Answer,
                NeedAns = DBfreqQ.NeedAns,
            };

            return Content(JsonConvert.SerializeObject(freqQInfo), "application/json");
        }
        #endregion

        #region Create New Form
        /// <summary>換頁時儲存表單資訊</summary>
        /// <param name="ajaxData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveInfoData(FormInfo ajaxData)
        {
            // Save into Session
            Session["FormInfo"] = ajaxData;

            // Set select List
            ViewBag.SelectList = DALFunctions.QusetionsType();

            FormLayout testLayout = new FormLayout();
            testLayout.Body = "test updated";

            // Set View Data
            ViewData["InfoData"] = (Session["FormInfo"] != null) ? Session["FormInfo"] : new FormInfo();
            ViewData["LayoutData"] = testLayout;

            return Content("InfoSaved");
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

            // Set relate date into layout model
            fLay.FormID = fid;
            fLay.QuestionSort = (Session["LayoutList"] == null) ? 1 : list.Count() + 1;

            // Add to list and save into session
            list.Add(fLay);
            Session["LayoutList"] = list;

            // generate html code and return to page
            return Content(ModelFunctions.LayoutListHTML(list));
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
        /// <summary>新常用問題資料寫入DB或更新資料</summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult EditFrequentQuestion(FrenquenQuestion q)
        {
            try
            {
                // check is edit or create new
                if(Session["FrequentIndex"] == null)
                {
                    // Add new line into DB
                    using (FormDBModel db = new FormDBModel())
                    {
                        db.FrenquenQuestions.Add(q);
                        db.SaveChanges();
                    }
                }
                else
                {
                    // Get ID order from session
                    int editID = Convert.ToInt32(Session["FrequentIndex"]);
                    // If Answer is no input, create null string for write into DB
                    string ansString = (q.Answer == null) ? "null" : $"'{q.Answer}'";

                    // update Question
                    using (FormDBModel db = new FormDBModel())
                    {
                        var updateQuestion = 
                            $@" 
                                UPDATE [dbo].[FrenquenQuestion]
                                SET
                                    [Name] = '{q.Name}',
                                    [Body] = '{q.Body}',
                                    [Answer] = {ansString},
                                    [QuestionType] = '{q.QuestionType}',
                                    [NeedAns] = '{q.NeedAns}'
                                WHERE [ID] = {editID}
                            ";
                        db.Database.ExecuteSqlCommand(updateQuestion);
                    }
                }

                return RedirectToAction("FrequentlyQuestions");
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

                return RedirectToAction("FrequentlyQuestions");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>顯示需要編輯的問題資料</summary>
        /// <param name="QID"></param>
        /// <returns></returns>
        public ActionResult ShowFrequentQuestions(int QID)
        {
            try
            {
                // Save ID into Seesion
                Session["FrequentIndex"] = QID;

                // Get data from db
                FrenquenQuestion dbData = new FormDBModel().FrenquenQuestions.Where(q => q.ID == QID).FirstOrDefault();

                ViewBag.SelectList = DALFunctions.QusetionsType(dbData.QuestionType);
                ViewBag.FrequenList = DALFunctions.FrequentlyQuestionsList();

                return View("FrequentlyQuestions", dbData);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }
        #endregion
    }
}