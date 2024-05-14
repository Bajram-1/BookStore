using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.DbConfig
{
    public class CategoriesConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Name)
                   .HasMaxLength(100)
                   .HasColumnName("Name")
                   .IsRequired();

            builder.Property(c => c.Description)
                   .HasMaxLength(500)
                   .HasColumnName("Description");

            builder.Property(c => c.DisplayOrder)
                   .HasColumnName("DisplayOrder")
                   .IsRequired();
        }
    }
}
