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
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(od => od.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(od => od.OrderHeader)
                .WithMany(oh => oh.OrderDetails)
                .HasForeignKey(od => od.OrderHeaderId);
        }
    }
}