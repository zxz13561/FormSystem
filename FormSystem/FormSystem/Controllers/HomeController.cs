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
            // Create New Form
            if (id == "NewForm")
            {
                CreateFormModel cModel = new CreateFormModel();
                cModel.mInfo = new FormInfo();
                cModel.mLayout = new FormLayout();

                return View(cModel);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditForm(CreateFormModel newForm)
        {
            return View();
        }
    }
}