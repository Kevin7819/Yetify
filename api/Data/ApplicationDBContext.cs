using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    /// Represents the database context for the application, providing access to entities
    public class ApplicationDBContext : DbContext
    {
        /// Initializes a new instance of the database context
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options){}

        /// Gets or sets the UserTasks table in the database
        public DbSet<UserTask> UserTasks { get; set; }

        /// Gets or sets the Courses table in the database
        public DbSet<Course> Courses { get; set; }

        /// Gets or sets the Users table in the database
        public DbSet<User> Users { get; set; }
    }
}