using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormSystem.DBModel;
using FormSystem.Models;
using FormSystem.Functions;
using Newtonsoft.Json;
using System.Web.Security;

namespace FormSystem.Controllers
{
    public class HomeController : Controller
    {
        #region Show Page Controllers
        /// <summary>首頁</summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>登入頁</summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            ViewBag.Message = "請輸入管理員帳號密碼";
            return View();
        }

        /// <summary>登入驗證</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            ViewBag.Message = "帳號密碼有誤，請重新輸入";

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (model.UserName != "admin" || model.Password != "12345")
                return View();

            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: "Admin",                                                 //可以放使用者Id
                issueDate: DateTime.UtcNow,                          //現在UTC時間
                expiration: DateTime.UtcNow.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
                isPersistent: true,                                              // 是否要記住我 true or false
                userData: "Admin",                                           //可以放使用者角色名稱
                cookiePath: FormsAuthentication.FormsCookiePath
            );

            var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的表單加密
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(cookie);

            return RedirectToAction("FormManager", "BackStage");
        }

        /// <summary>前台分析頁</summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public ActionResult FrontAnalysis(Guid FormID)
        {
            Session["FID"] = FormID;
            int countData = DALFunctions.GetFormAns(FormID).Count();
            ViewData["notEmpty"] = countData > 0 ? true : false;
            return View();
        }

        /// <summary>錯誤頁</summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public ActionResult ErrorPage(string errMsg)
        {
            ViewBag.ErrMsg = errMsg;
            return View();
        }
        #endregion

        #region Pagination Controllers
        /// <summary>首頁分頁控制</summary>
        /// <param name="fLay"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IndexPagination()
        {
            // Count how many item in DB
            int totalCount = new FormDBModel().FormInfoes.ToList().Count();
            int itemsPerPage = 5;

            // send html code
            IndexPagerModel indexHTML = new IndexPagerModel()
            {
                ShowFormsHTML = ModelFunctions.IndexListHTML(),  // questions list
                PagniationHTML = ModelFunctions.PaginationHTML(totalCount, itemsPerPage), // pagination list
                MaxPage = totalCount % itemsPerPage == 0 ? totalCount / itemsPerPage : totalCount / itemsPerPage + 1, // max page number
                ItemsPerPage = itemsPerPage
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
                    ViewBag.FormStart = formInfo.StartDate.ToString("yyyy-MM-dd");
                    ViewBag.FormEnd = formInfo.EndDate.ToString("yyyy-MM-dd");

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

                // Get layout list from session
                List<FormLayout> fLayout = (List<FormLayout>)Session["FillFormInfo"];

                // Set answer list
                List<ShowAnsModel> showAns = new List<ShowAnsModel>();

                // Set new model contain answer for DB
                FormData sessionAns = new FormData() { FormID = fillFid};

                for (int i = 0; i < fLayout.Count; i++)
                {
                    // Record Layout order
                    sessionAns.QuestionSort += fLayout[i].QuestionType + ";";

                    // Record user input Answer order
                    sessionAns.AnswerData += (ansList[i].Answer.Count() > 1) ? string.Join(",", ansList[i].Answer) : ansList[i].Answer[0]; // Check is single answer or mutiple
                    sessionAns.AnswerData += ";";

                    // Create a string filt out false answer
                    string ansStr = (ansList[i].Answer.Count() > 1) ? string.Join(",", ansList[i].Answer.Where((source, value) => source != "false").ToArray()) : ansList[i].Answer[0];

                    // Create a object combine answer and layout, Add into list
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
                    throw new Exception("Session Data is null");

                // Get data from session
                FormData fData =  (FormData)Session["FormData"];
                fData.CreateDate = DateTime.Now;

                // insert into DB
                DALFunctions.AnsInsertDB(fData);

                return View("SuccessPage");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }
        #endregion

        #region Form Answer Analysis
        /// <summary>顯示表單答案的詳細資料</summary>
        /// <param name="fid"></param>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public ActionResult HistoryAns(string fid, string dataID)
        {
            try
            {
                // Check id is correct
                if (!Guid.TryParse(fid, out Guid FID))
                    throw new Exception("GUID Error");

                if (!int.TryParse(dataID, out int _dataID))
                    throw new Exception("Data ID Error");

                // Set objects
                List<ShowAnsModel> ansInfoList = new List<ShowAnsModel>();
                List<FormLayout> layoutList = DALFunctions.GetFormLayoutByFID(FID);
                FormData fData = DALFunctions.GetFormAnsInfo(_dataID);

                // Re-format data
                string[] ansArray = fData.AnswerData.Split(';');
                string[] QtypeArray = fData.QuestionSort.Split(';');

                // Putting it together
                for (int i = 0; i < layoutList.Count(); i++)
                {
                    ansInfoList.Add(new ShowAnsModel()
                    {
                        QBody = layoutList[i].Body,
                        QAns = ansArray[i]
                    });
                }

                return View(ansInfoList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>提供表單分析頁面中的圖表資料</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PlotData()
        {
            try
            {
                // Check FormID from session
                if (!Guid.TryParse(Session["FID"].ToString(), out Guid FID))
                    throw new Exception("GUID Error");

                // Set objects
                Dictionary<string, AnsObject[]> outputList = new Dictionary<string, AnsObject[]>();
                List<string[]> ansArrayList = new List<string[]>();             

                // Get data from DB
                List<FormLayout> layoutList = DALFunctions.GetSpecficLayout(FID);
                List<FormData> dbAnsList = DALFunctions.GetFormAns(FID);

                // Set parameter
                int howManyQuestions = layoutList.Count();
                bool[] isMultiple = new bool[howManyQuestions];

                // Loop each item and set object list
                for (int layIndex = 0; layIndex < howManyQuestions; layIndex++)
                {
                    string[] ansOptions = layoutList[layIndex].Answer.Split(';');
                    int howManyAns = ansOptions.Count();
                    AnsObject[] tempObject = new AnsObject[howManyAns];

                    // Check is multiple chose
                    if (layoutList[layIndex].QuestionType == "QT06 " || layoutList[layIndex].QuestionType == "QT06")
                        isMultiple[layIndex] = true;

                    // Set data into temp array
                    for (int i = 0; i < howManyAns; i++)
                    {
                        tempObject[i] = new AnsObject() { label = ansOptions[i], data = 0 };
                    }

                    // Add array into output list
                    outputList.Add(layoutList[layIndex].Body, tempObject);
                }

                // Loop answer data to string array
                foreach (var a in dbAnsList)
                {
                    string[] ansDataArray = a.AnswerData.Split(';');
                    string[] newAnsArray = new string[howManyQuestions];

                    // Only select data what we need
                    for (int i = 0; i < howManyQuestions; i++)
                        newAnsArray[i] = ansDataArray[layoutList[i].QuestionSort - 1];

                    ansArrayList.Add(newAnsArray);
                }

                // Loop all questions
                for (int Qindex = 0; Qindex < outputList.Count(); Qindex++)
                {
                    // Chose loop by question type
                    if (isMultiple[Qindex])
                    {
                        // Loop all answers
                        for (int Aindex = 0; Aindex < ansArrayList.Count(); Aindex++)
                        {
                            // Split multiple answear into array
                            string[] ansArr = ansArrayList[Aindex][Qindex].Split(',');

                            // Loop all select options
                            for (int selsectIndex = 0; selsectIndex < ansArr.Count(); selsectIndex++)
                            {
                                // Check is chosed
                                if (ansArr[selsectIndex] != "false")
                                    outputList.ElementAt(Qindex).Value[selsectIndex].data += 1;
                            }
                        }
                    }
                    else
                    {
                        // Loop all answers
                        foreach (var ansArr in ansArrayList)
                        {
                            foreach (AnsObject ans in outputList.ElementAt(Qindex).Value)
                            {
                                if (ansArr[0] == ans.label)
                                {
                                    ans.data += 1;
                                    break; // break foreach loop
                                }
                            }
                        }
                    }
                }

                return Json(outputList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }
        }

        /// <summary>從DB獲得表單資料後整理回傳前端</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportCSV()
        {
            try
            {
                // Check FormID from session
                if (!Guid.TryParse(Session["FID"].ToString(), out Guid FID))
                    throw new Exception("GUID Error");

                // Get data from DB
                List<FormLayout> layoutList = DALFunctions.GetFormLayoutByFID(FID);
                List<FormData> dbAnsList = DALFunctions.GetFormAns(FID);

                // Set return objects
                List<string[]> exportArray = new List<string[]>();

                // set columns name
                string[] tempLayout = new string[layoutList.Count()];
                for(int i = 0; i < layoutList.Count(); i++)
                    tempLayout[i] = layoutList[i].Body;

                // add layout string array
                exportArray.Add(tempLayout);

                // set ans into array
                foreach(var dbAns in dbAnsList)
                {
                    string[] ansArr = dbAns.AnswerData.Split(';');
                    exportArray.Add(ansArr);                           
                }

                return Json(exportArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Home", new { errMsg = ex.ToString() });
            }           
        }
        #endregion

    }
}