using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentInfoLoginRoles.Models
{
    public class ApplicationContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<FeeTransaction> FeeTransactions { get; set; } = null!;
        public DbSet<CourseDetails> CourseDetails { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>()
                .HasMany(s => s.FeeTransactions)
                .WithOne(ft => ft.Student)
                .HasForeignKey(ft => ft.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FeeTransaction>()
                .HasOne(f => f.Student)
                .WithMany(s => s.FeeTransactions)
                .HasForeignKey(f => f.StudentId);

            modelBuilder.Entity<FeeTransaction>()
                .Property(f => f.TransactionDate)
                .HasColumnType("date");

            modelBuilder.Entity<CourseDetails>()
                .HasMany(cd => cd.Students)
                .WithOne()
                .HasForeignKey(s => s.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
