namespace FormSystem.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FormLayout")]
    public partial class FormLayout
    {
        public int ID { get; set; }

        public Guid FormID { get; set; }

        [Required]
        [StringLength(10)]
        public string QuestionType { get; set; }

        [Required]
        [StringLength(50)]
        public string Body { get; set; }

        [StringLength(50)]
        public string Answer { get; set; }

        public bool NeedAns { get; set; }

        public virtual FormInfo FormInfo { get; set; }

        public virtual QuestionType QuestionType1 { get; set; }
    }
}
