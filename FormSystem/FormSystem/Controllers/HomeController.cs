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

        public ActionResult FormInfo()
        {
            ViewBag.Message = "Put Form info here.";

            return View();
        }

        public ActionResult FillForm(string id)
        {
            // check id is correct
            ViewBag.Message = id;
            if(!Guid.TryParse(id, out Guid FID))
            {
                ViewBag.FormTitle = "發生錯誤";
                ViewBag.FormBody = "請回上一頁重新選取";
                ViewBag.FormStart = "";
                ViewBag.FormEnd = "";

                return View();
            }

            using (FormDBModel dbModel = new FormDBModel())
            {
                // Get Form information
                var formInfo = dbModel.FormInfoes
                        .Where(info => info.FormID == FID)
                        .FirstOrDefault();

                ViewBag.FormTitle = formInfo.Name;
                ViewBag.FormBody = formInfo.Body;
                ViewBag.FormStart = formInfo.StartDate.ToString("yyyy-mm-dd hh:mm:ss");
                ViewBag.FormEnd = formInfo.EndDate.ToString("yyyy-mm-dd hh:mm:ss");

                // Get Form layout and sent to view page
                var formLayout = new FormDBModel().FormLayouts
                        .Where(layout => layout.FormID == FID)
                        .OrderBy(layout => layout.ID);

                return View(formLayout.ToList());
            }
        }

        [HttpGet]
        public ActionResult CheckAns(List<FormLayout> ansModel)
        {
            return View(ansModel);
        }

        public ActionResult FormManager()
        {

            return View();
        }

        public ActionResult FrequentlyQuestions()
        {

            return View();
        }

        #region Edit Forms


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

                return View(cModel);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditFormInfo(FormInfo fInfo)
        {
            // Save into Session
            Session["FormInfo"] = fInfo;

            // collect data and return
            ViewBag.SelectList = ModelFunctions.QusetionsType();
            CreateFormModel cModel = new CreateFormModel() { mInfo = fInfo, mLayout = new FormLayout() };
            return View("EditForm", cModel);
        }

        [HttpPost]
        public ActionResult EditFormLayout(FormLayout fLay)
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

            /*
            string htmlText = string.Empty;

            foreach (var q in list)
            {
                htmlText += $@"
                <tr>
                    <td><input type=""checkbox"" id=""chkbox{q.ID}"" value=""{q.ID}"" checked=""false""></td>
                    <td>{q.QuestionSort}</td>
                    <td>{q.Body}</td>
                    <td>{q.QuestionType}</td>
                    <td><a href=""Home/Index/{q.ID}"">編輯</a></td>
                </tr>";
            }
            */

            // collect data and return
            ViewBag.SelectList = ModelFunctions.QusetionsType();
            FormInfo fInfo = (Session["FormInfo"] != null) ? (FormInfo)Session["FormInfo"] : new FormInfo();
            CreateFormModel cModel = new CreateFormModel() { mInfo = fInfo, mLayout = new FormLayout() };
            return View("EditForm", cModel);
        }

        [HttpGet]
        public ActionResult GetLayoutQuestions()
        {
            List<FormLayout> list = (List<FormLayout>)Session["LayoutList"];
            
            
            string htmlText = string.Empty;

            foreach(var q in list)
            {
                htmlText += $@"
                <tr>
                    <td>{q.QuestionSort}</td>
                    <td>{q.Body}</td>
                    <td>{q.QuestionType}</td>
                    <td>@Html.ActionLink(""Home"", ""Index"", new {{ @id = {q.ID}}})</td>
                </tr>";
            }
            return Json(htmlText);
        }

        public ActionResult _LayoutPartial()
        {
            return PartialView(new FormLayout());
        }

        [HttpPost]
        public ActionResult LayoutData()
        {
            return PartialView(new FormLayout());
        }
        #endregion
    }
}