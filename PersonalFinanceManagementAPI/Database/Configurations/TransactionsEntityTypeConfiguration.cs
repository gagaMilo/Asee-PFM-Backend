using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManagementAPI.Database.Entities;


namespace PersonalFinanceManagementAPI.Database.Configurations
{
    public class TransactionsEntityTypeConfiguration : IEntityTypeConfiguration<TransactionsEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionsEntity> builder)
        {
            //dizajn tabele
            builder.ToTable("transactions");
            //primarni kljuc - za vezivanje dve tabele (najcesce)
            builder.HasKey(x => x.Id);
            //definisem kolone
            builder.Property(x => x.Id).IsRequired().HasMaxLength(32).ValueGeneratedOnAdd();
            builder.Property(x => x.BeneficiaryName).HasMaxLength(64);
            builder.Property(x => x.Date).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Direction).HasConversion<string>().IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1024);
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            builder.Property(x => x.Mcc); 
            builder.Property(x => x.Kind).HasConversion<string>().IsRequired();
            

            // Konverter za Splits property
            // Konvertor za Splits svojstvo
            /*
            var splitListConverter = new SingleCategorySplitListConverter();
            builder.Property(x => x.Splits)
                .HasConversion(splitListConverter);

            */
            //readOnly 
            // builder.Property(x => x.Id).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
            // builder.Property(x => x.Catcode).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
            /*
            builder.HasOne(x => x.Category)
                       .WithMany() 
                       .HasForeignKey(x => x.Catcode)
                       .OnDelete(DeleteBehavior.Restrict); 
            */
        }
    }
}
