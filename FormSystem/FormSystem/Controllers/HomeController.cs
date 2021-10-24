using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormSystem.DBModel;
using FormSystem.Models;

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

        public ActionResult EditForm(string id)
        {
            // Import Question Type Datas to DropdownList
            var QTypeList = new List<SelectListItem>() {};
            var typeQuery = new FormDBModel().QuestionTypes;

            foreach (var type in typeQuery)
            {
                QTypeList.Add(new SelectListItem { Text = type.Name, Value = type.TypeID });
            }
            ViewBag.SelectList = QTypeList;

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
        public ActionResult EditFormInfo(CreateFormModel newForm)
        {
            Guid emptyFID = new Guid("00000000-0000-0000-0000-000000000000");

            if (newForm.mInfo.FormID == emptyFID)
            {
                return RedirectToAction("FormManager", "Home");
            }

            return RedirectToAction("FormManager", "Home");
        }

        [HttpPost]
        public ActionResult EditFormLayout(CreateFormModel m)
        {
            // Check session data exist
            List<FormLayout> list = (Session["LayoutList"] != null) ? (List<FormLayout>)Session["LayoutList"] : new List<FormLayout>();
            Guid.TryParse(Session["FID"].ToString(), out Guid fid);

            FormLayout questionInfo = new FormLayout() {
                FormID = fid,
                Body = m.mLayout.Body,
                Answer = m.mLayout.Answer,
                QuestionType = m.mLayout.QuestionType,
                NeedAns = m.mLayout.NeedAns,
                QuestionSort = (Session["LayoutList"] == null) ? 0 : list.Count()
            };

            list.Add(questionInfo);
            Session["LayoutList"] = list;
            //ViewBag.layouts = list;

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

            return Json(htmlText);
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
    }
}