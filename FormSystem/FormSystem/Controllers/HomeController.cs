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

        public ActionResult FrequentlyQuestions()
        {
            return View();
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
                        fInfo.CreateDate = DateTime.Now;
                        db.FormInfoes.Add(fInfo);

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

        public ActionResult DeleteForm(string[] chkForm)
        {
            try
            {
                using (FormDBModel db = new FormDBModel())
                {
                    foreach (string _fid in chkForm)
                    {
                        if(_fid != "false"){
                            if (!Guid.TryParse(_fid, out Guid FormID))
                                throw new Exception("FID Error");

                            var infoDB =
                                from info in db.FormInfoes
                                where info.FormID == FormID
                                select info;

                            var layoutDB =
                                from layout in db.FormLayouts
                                where layout.FormID == FormID
                                select layout;

                            var dataDB =
                                from data in db.FormDatas
                                where data.FormID == FormID
                                select data;
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("FormManager");
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
                QuestionSort = (Session["LayoutList"] == null) ? 0 : list.Count()
            };

            list.Add(questionInfo);
            Session["LayoutList"] = list;
            ViewBag.layouts = list;

            int i = 1; //Question table index number
            string tableString = string.Empty;
            
            foreach (var data in list)
            {
                tableString += $@"
                    <tr>
                        <td><input type=""checkbox"" id=""{data.Body}"" name=""{data.Body}"" value=""{data.ID}""></td>
                        <td>{i}</td>
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

    }
}