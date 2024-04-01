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
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.HasKey(od => od.Id);

            builder.Property(od => od.OrderHeaderId)
                .IsRequired();

            builder.Property(od => od.ProductId)
                .IsRequired();

            builder.Property(od => od.Count)
                .IsRequired();

            builder.Property(od => od.Price)
                .IsRequired();

            builder.HasOne(od => od.OrderHeader)
                .WithOne()
                .HasForeignKey<OrderDetail>(od => od.OrderHeaderId);

            builder.HasOne(od => od.Product)
                .WithOne()
                .HasForeignKey<OrderDetail>(od => od.ProductId);
        }
    }
}