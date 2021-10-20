using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormSystem.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
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
            ViewBag.Message = id;

            return View();
        }

        public ActionResult FormManager()
        {

            return View();
        }

        public ActionResult FrequentlyQuestions()
        {

            return View();
        }

        public ActionResult EditForm()
        {

            return View();
        }
    }
}