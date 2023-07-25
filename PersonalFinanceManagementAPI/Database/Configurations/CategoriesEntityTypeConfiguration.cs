using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagementAPI.Database.Entities;

namespace PersonalFinanceManagementAPI.Database.Configurations
{
    public class CategoriesEntityTypeConfiguration : IEntityTypeConfiguration<CategoriesEntity>
    {
        public void Configure(EntityTypeBuilder<CategoriesEntity> builder)
        {
            //dizajn tabele
            builder.ToTable("categories");
            //primarni kljuc - za vezivanje dve tabele (najcesce)
            builder.HasKey(x => x.Code);
            //definisem kolone
            builder.Property(x => x.Code).IsRequired().HasMaxLength(3).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasMaxLength(32);
            builder.Property(x => x.ParentCode).HasMaxLength(1);
        }
    }
}

