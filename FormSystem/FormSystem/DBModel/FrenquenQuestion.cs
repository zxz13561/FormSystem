namespace FormSystem.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FrenquenQuestion")]
    public partial class FrenquenQuestion
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Body { get; set; }

        [StringLength(50)]
        public string Answer { get; set; }

        [Required]
        [StringLength(10)]
        public string QuestionType { get; set; }

        public bool NeedAns { get; set; }

        public virtual QuestionType QuestionType1 { get; set; }
    }
}
