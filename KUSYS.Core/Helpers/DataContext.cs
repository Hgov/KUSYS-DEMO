using KUSYS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Core.Helpers
{
    public class DataContext : IdentityDbContext<User, Role, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCourse>().HasKey(sc => new { sc.UserId, sc.CourseId });

            //Role Insert

            modelBuilder.Entity<Role>().HasData(new Role { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "ADMIN".ToUpper() });
            modelBuilder.Entity<Role>().HasData(new Role { Id = "2k5e174e-3b0e-446f-86af-483d56fd7211", Name = "Student", NormalizedName = "STUDENT".ToUpper() });
            //User Insert
            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    FirstName="TestAdmin1",
                    UserName = "testadmin1",
                    NormalizedUserName = "TESTADMIN1",
                    Email = "testadmin1@kusys.com",
                    NormalizedEmail = "TESTADMIN1@KUSYS.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Q.werty123")
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cd10",
                    FirstName = "TestStudent",
                    UserName = "teststudent1",
                    NormalizedUserName = "TESTSTUDENT1",
                    Email = "teststudent1@kusys.com",
                    NormalizedEmail = "TESTSTUDENT1@KUSYS.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Q.werty123")
                });

            //user assignment role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
               new IdentityUserRole<string>
               {
                   RoleId = "2k5e174e-3b0e-446f-86af-483d56fd7211",
                   UserId = "8e445865-a24d-4543-a6c6-9443d048cd10"
               }
           );

            //Course Insert
            modelBuilder.Entity<Course>()
                        .HasData(
                         new Course { CourseId = "6y445865-a24d-4543-a6c6-9443d048cd10", CourseCode = "CSI101", Name = "Introduction to Computer Science" },
                         new Course { CourseId = "6y445865-a24d-4543-a6c6-9443d048cd11", CourseCode = "CSI102", Name = "Algorithms" },
                         new Course { CourseId = "6y445865-a24d-4543-a6c6-9443d048cd12", CourseCode = "MAT101", Name = "Calculus" },
                         new Course { CourseId = "6y445865-a24d-4543-a6c6-9443d048cd13", CourseCode = "PHY101", Name = "Physics" }
                         );
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
    }
}
