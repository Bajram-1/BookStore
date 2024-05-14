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
    public class ProductsConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(e => e.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Title)
                .IsRequired();

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.ISBN)
                .IsRequired();

            builder.Property(p => p.Author)
                .IsRequired();

            builder.Property(p => p.ListPrice)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(p => p.Price50)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(p => p.Price100)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey("ProductId");

            //builder.HasMany(p => p.OrderDetails)
            //   .WithOne(od => od.Product)
            //   .HasForeignKey(od => od.ProductId);
        }
    }
}
