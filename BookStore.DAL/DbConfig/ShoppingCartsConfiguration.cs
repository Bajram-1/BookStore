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
    public class ShoppingCartsConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCarts");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.ProductId)
                .IsRequired();

            builder.Property(sc => sc.Count)
                .IsRequired();

            builder.Property(sc => sc.ApplicationUserId)
                .IsRequired();

            builder.HasOne(sc => sc.Product)
                .WithOne()
                .HasForeignKey<ShoppingCart>(sc => sc.ProductId);

            builder.HasOne(sc => sc.ApplicationUser)
                .WithOne()
                .HasForeignKey<ShoppingCart>(sc => sc.ApplicationUserId);
        }
    }
}