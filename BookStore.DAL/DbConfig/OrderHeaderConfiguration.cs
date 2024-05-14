using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.DbConfig
{
    public class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
    {
        public void Configure(EntityTypeBuilder<OrderHeader> builder)
        {
            builder.ToTable("OrderHeaders");

            builder.HasKey(oh => oh.Id);

            builder.Property(oh => oh.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(oh => oh.ApplicationUserId)
                .IsRequired();

            builder.Property(oh => oh.OrderDate)
                .IsRequired();

            builder.Property(oh => oh.ShippingDate)
                .IsRequired();

            builder.Property(oh => oh.OrderTotal)
                .IsRequired()
                .HasPrecision(18,2);

            builder.Property(oh => oh.OrderStatus)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.PaymentStatus)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.TrackingNumber)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.Carrier)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.PaymentDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(oh => oh.PaymentDueDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(oh => oh.SessionId)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.PaymentIntentId)
                .HasColumnType("nvarchar(max)");

            builder.Property(oh => oh.PhoneNumber)
                .IsRequired();

            builder.Property(oh => oh.StreetAddress)
                .IsRequired();

            builder.Property(oh => oh.City)
                .IsRequired();

            builder.Property(oh => oh.State)
                .IsRequired();

            builder.Property(oh => oh.PostalCode)
                .IsRequired();

            builder.Property(oh => oh.Name)
                .IsRequired();

            builder.HasOne(oh => oh.ApplicationUser)
             .WithMany(au => au.OrderHeaders)
             .HasForeignKey(oh => oh.ApplicationUserId);
        }
    }
}
