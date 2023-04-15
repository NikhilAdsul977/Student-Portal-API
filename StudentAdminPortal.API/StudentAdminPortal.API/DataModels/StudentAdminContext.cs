using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentAdminPortal.API.DataModels
{
    public class StudentAdminContext : DbContext
    {
        public StudentAdminContext(DbContextOptions<StudentAdminContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }

        public DbSet<Gender> Genders { get; set; }

        public DbSet<Address> Address { get; set; }
    }

    //public class StudentAdminContextFactory : IDesignTimeDbContextFactory<StudentAdminContext>
    //{
    //    public StudentAdminContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<StudentAdminContext>();
    //        optionsBuilder.UseSqlServer("your connection string");

    //        return new StudentAdminContext(optionsBuilder.Options);
    //    }

 
    //}
}
