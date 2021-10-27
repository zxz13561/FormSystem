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
            Session["FillFormFID"] = id;
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
                ViewBag.FormStart = formInfo.StartDate.ToString("yyyy-MM-dd hh:mm:ss");
                ViewBag.FormEnd = formInfo.EndDate.ToString("yyyy-MM-dd hh:mm:ss");

                // Get Form layout and sent to view page
                var formLayout = new FormDBModel().FormLayouts
                        .Where(layout => layout.FormID == FID)
                        .OrderBy(layout => layout.ID);

                return View(formLayout.ToList());
            }
        }

        public ActionResult CheckAns(List<FormAnsModel> ansList)
        {
            try
            {
                if(!Guid.TryParse(Session["FillFormFID"].ToString(), out Guid fillFid))
                {
                    return new HttpStatusCodeResult(500, "ERROR");
                }

                List<string> reviewAns = new List<string>();

                foreach(FormAnsModel ansArr in ansList)
                {                    
                    string ans = (ansArr.Answer.Count() > 1) ? string.Join(",", ansArr.Answer.Where((source, value) => source != "false").ToArray()) : ansArr.Answer[0];                    
                    reviewAns.Add(ans);
                }

                return View(reviewAns);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.ToString());
            }
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

        #region Create New Form
        [HttpPost]
        public ActionResult NewInfo(FormInfo fInfo)
        {
            // Save into Session
            Session["FormInfo"] = fInfo;

            return Content("資料儲存成功");
        }

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
                    return new HttpStatusCodeResult(500, "DB Err");
                }

                return RedirectToAction("FormManager", "Home");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "DB Err");
            }
        }
    }
}