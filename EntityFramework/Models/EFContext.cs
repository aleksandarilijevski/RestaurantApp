using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Models
{
    public class EFContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Restaurant;Integrated Security=true");
        }

        public DbSet<Test> Tests { get; set; }
    }
}
