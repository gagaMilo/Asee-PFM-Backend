using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagementAPI.Database.Configurations;
using PersonalFinanceManagementAPI.Database.Entities;

namespace PersonalFinanceManagementAPI.Database
{
    public class PFMDbContext : DbContext
    {
        public DbSet<TransactionsEntity> Transactions { get; set; }
        public DbSet<CategoriesEntity> Categories { get; set; }
        
        public DbSet<SplitsEntity> SplitsTransactions { get; set; }
      
        public PFMDbContext(DbContextOptions options) : base(options)
        {
        }

        public PFMDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            modelBuilder.ApplyConfiguration(
                //transactions
              new TransactionsEntityTypeConfiguration()
            );
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            modelBuilder.ApplyConfiguration(
              //categories
              new CategoriesEntityTypeConfiguration()
            );
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            modelBuilder.ApplyConfiguration(
             //splitsTransactions
             new SplitsTransactionsEntityTypeConfiguration()
           );
            


           
            base.OnModelCreating(modelBuilder);
        }
    }
}
