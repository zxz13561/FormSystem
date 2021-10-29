using FormSystem.DBModel;
using FormSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormSystem.Functions
{
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
    }
}