using Microsoft.EntityFrameworkCore;

namespace SchemaStar.Models
{
    public class UserDbContext : DbContext
    {
        //inherit DbContext Class
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<User> User { get; set; } = null!;
    }
}
