using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagementAPI.Database.Entities;
using System.Reflection.Emit;

namespace PersonalFinanceManagementAPI.Database.Configurations
{
    public class SplitsTransactionsEntityTypeConfiguration : IEntityTypeConfiguration<SplitsEntity>
    {
        public void Configure(EntityTypeBuilder<SplitsEntity> builder)
        {
            //dizajn tabele
            builder.ToTable("splitsTransactions");

            //primarni kljuc - za vezivanje dve tabele (najcesce)
        
            builder.HasKey(st => new { st.TransactionId, st.Catcode });

            //definisem kolone
            builder.Property(x => x.TransactionId);
            builder.Property(x => x.Catcode);
            builder.Property(x => x.Amount);


            builder.HasOne(x => x.transaction)
                .WithMany(s => s.Splits);

            builder.HasOne(x => x.category)
               .WithMany(s => s.Splits);
        }
    }
}
