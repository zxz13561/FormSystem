namespace FormSystem.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FormData")]
    public partial class FormData
    {
        [Key]
        public int DataID { get; set; }

        public Guid FormID { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreateDate { get; set; }

        public string AnswerData { get; set; }

        public string QuestionSort { get; set; }

        public virtual FormInfo FormInfo { get; set; }
    }
}
