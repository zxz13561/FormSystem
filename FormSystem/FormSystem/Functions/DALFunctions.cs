using FormSystem.DBModel;
using FormSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormSystem.Functions
{
    // Data Access Level
    public class DALFunctions
    {
        /// <summary>從DB獲取FrequentlyQuestions資料，整理成List回傳</summary>
        /// <returns></returns>
        public static List<FrequenModel> FrequentlyQuestionsList()
        {
            try
            {
                // Create New List
                List<FrequenModel> frquenM = new List<FrequenModel>();

                using (FormDBModel db = new FormDBModel())
                {
                    // get data from db
                    var result = (
                        from f in db.FrenquenQuestions
                        join q in db.QuestionTypes on f.QuestionType equals q.TypeID
                        select new
                        {
                            ID = f.ID,
                            Name = f.Name,
                            Body = f.Body,
                            QuestionType = q.Name,
                            NeedAns = f.NeedAns
                        }
                    );

                    // write data into list
                    foreach (var item in result.ToList())
                    {
                        frquenM.Add(new FrequenModel()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Body = item.Body,
                            QuestionType = item.QuestionType,
                            NeedAns = item.NeedAns ? "必填" : "非必填"
                        });
                    }
                }

                return frquenM;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>從DB獲取問題種類下拉選單</summary>
        /// <returns></returns>
        public static List<SelectListItem> QusetionsType(string beenSelected = null)
        {
            // Import Question Type Datas to DropdownList
            List<SelectListItem> QTypeList = new List<SelectListItem>() { };
            var typeQuery = new FormDBModel().QuestionTypes;

            foreach (var type in typeQuery)
            {
                // bypass space charater
                string[] dbType = type.TypeID.Split(' ');
                string[] dataType = (beenSelected != null) ? beenSelected.Split(' ') : null;

                // set which is selected
                if (dataType != null && dataType[0] == dbType[0])
                    QTypeList.Add(new SelectListItem { Text = type.Name, Value = type.TypeID, Selected = true });
                else
                    QTypeList.Add(new SelectListItem { Text = type.Name, Value = type.TypeID });
            }

            return QTypeList;
        }

        /// <summary>從DB獲取常用問題下拉選單</summary>
        /// <param name="beenSelected"></param>
        /// <returns></returns>
        public static List<SelectListItem> frequenQList(string beenSelected = null)
        {
            List<SelectListItem> qList = new List<SelectListItem>() { };
            var frequentQs = new FormDBModel().FrenquenQuestions;

            // Set first select item is self modify
            qList.Add(new SelectListItem { Text = "自訂問題", Value = "0", Selected = true });

            foreach (var q in frequentQs)
            {
                qList.Add(new SelectListItem { Text = q.Name.ToString(), Value = q.ID.ToString() });
            }

            return qList;
        }

        /// <summary>依照ID從DB篩選FrequentlyQuestions資料並回傳</summary>
        /// <returns></returns>
        public static FrenquenQuestion GetFreQInfo(int FreQID)
        {
            try
            {
                FrenquenQuestion getQ = new FrenquenQuestion();

                using (FormDBModel db = new FormDBModel())
                {
                    // get data from db
                    var result =
                        from f in db.FrenquenQuestions
                        where f.ID == FreQID
                        select f;

                    getQ = result.FirstOrDefault();
                }
                return getQ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>依照FormID從DB篩選FormInfo資料並回傳</summary>
        /// <param name="_fid"></param>
        /// <returns></returns>
        public static FormInfo GetFormInfoByFID(Guid _fid)
        {
            try
            {
                FormInfo _fInfo = new FormInfo() { };

                using (FormDBModel db = new FormDBModel())
                {
                    // get data from db
                    var result =
                        from f in db.FormInfoes
                        where f.FormID == _fid
                        select f;

                    _fInfo = result.FirstOrDefault();
                }

                return _fInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>依照FormID從DB篩選FormLayout資料並回傳</summary>
        /// <param name="_fid"></param>
        /// <returns></returns>
        public static List<FormLayout> GetFormLayoutByFID(Guid _fid)
        {
            try
            {
                List<FormLayout> _flayout = new List<FormLayout>() { };

                using (FormDBModel db = new FormDBModel())
                {
                    // get data from db
                    var result =
                        from f in db.FormLayouts
                        where f.FormID == _fid
                        select f;

                    foreach(var item in result.ToList())
                    {
                        _flayout.Add(item);
                    }
                }

                return _flayout;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>取得問題類型名稱</summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GetQuestionTypeName(string ID)
        {
            try
            {
                QuestionType getType = new QuestionType();

                using (FormDBModel db = new FormDBModel())
                {
                    // get data from db
                    var result =
                        from type in db.QuestionTypes
                        where type.TypeID == ID
                        select type;

                    getType = result.FirstOrDefault();
                }
                return getType.Name;
            }
            catch (Exception)
            {
                return "Error";
            }
        }
    }
}