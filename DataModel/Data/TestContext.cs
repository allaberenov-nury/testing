using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InnovationTest.Models;

namespace InnovationTest.Data
{
    public class InnovationTestContext : DbContext
    {
        public InnovationTestContext (DbContextOptions<InnovationTestContext> options)
            : base(options)
        {
        }

        public DbSet<TestName> TestName { get; set; }

        

        public DbSet<Student> Student { get; set; }

        public DbSet<University> University { get; set; }

        public DbSet<AnswerDetail> AnswerDetail { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestResult>()
                .HasMany<AnswerDetail>(y => y.AnswerDetail)
                .WithOne(s => s.TestResult)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<AnswerDetail>()
                .HasOne(x => x.Question)
                .WithMany(s => s.AnswerDetails)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
