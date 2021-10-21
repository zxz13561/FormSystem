using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace FormSystem.DBModel
{
    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<FormData> FormDatas { get; set; }
        public virtual DbSet<FormInfo> FormInfoes { get; set; }
        public virtual DbSet<FormLayout> FormLayouts { get; set; }
        public virtual DbSet<FrenquenQuestion> FrenquenQuestions { get; set; }
        public virtual DbSet<QuestionType> QuestionTypes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FormInfo>()
                .HasMany(e => e.FormDatas)
                .WithRequired(e => e.FormInfo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormInfo>()
                .HasMany(e => e.FormLayouts)
                .WithRequired(e => e.FormInfo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<QuestionType>()
                .HasMany(e => e.FormLayouts)
                .WithRequired(e => e.QuestionType1)
                .HasForeignKey(e => e.QuestionType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<QuestionType>()
                .HasMany(e => e.FrenquenQuestions)
                .WithRequired(e => e.QuestionType1)
                .HasForeignKey(e => e.QuestionType)
                .WillCascadeOnDelete(false);
        }
    }
}
