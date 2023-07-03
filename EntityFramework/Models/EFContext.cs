using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Models
{
    public class EFContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Restaurant;Integrated Security=true");
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Table> Tables { get; set; }

        public DbSet<Waiter> Waiters { get; set; }

        public DbSet<Bill> Bills { get; set; }
    }
}
