using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XayahBot.Database.Model;

namespace XayahBot.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("XayahBot.Database.Model.TProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("XayahBot.Database.Model.TQuizStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Answers");

                    b.Property<ulong>("Guild");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.ToTable("QuizStats");
                });
        }
    }
}
