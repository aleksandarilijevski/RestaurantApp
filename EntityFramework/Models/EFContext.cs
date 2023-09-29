using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Models
{
    public class EFContext : DbContext
    {
        public EFContext()
        {
            
        }

        public EFContext(DbContextOptions options) : base(options)
        {

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (EntityEntry entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDateTime = DateTime.Now;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    ((BaseEntity)entityEntry.Entity).ModifiedDateTime = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellation);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Restaurant;Integrated Security=true");
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableArticleQuantity>().UseTpcMappingStrategy();
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Table> Tables { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Bill> Bills { get; set; }

        public DbSet<ArticleDetails> ArticleDetails { get; set; }

        public DbSet<DataEntry> DataEntries { get; set; }

        public DbSet<TableArticleQuantity> TableArticleQuantities { get; set; }

        public DbSet<SoldTableArticleQuantity> SoldTableArticleQuantities { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<OnlineOrder> OnlineOrders { get; set; }

        public DbSet<SoldArticleDetails> SoldArticleDetails { get; set; }
    }
}
