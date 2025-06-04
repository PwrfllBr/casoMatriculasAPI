using Microsoft.EntityFrameworkCore;

namespace casoMatriculasAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Student> Students { get; set; } = null!;
        public DbSet<Models.Course> Courses { get; set; } = null!;
        public DbSet<Models.Enrollment> Enrollments { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // A student cannot enroll in the same course more than once
            modelBuilder.Entity<Models.Enrollment>()
                .HasIndex(e => new { e.IdStudent, e.IdCourse })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
