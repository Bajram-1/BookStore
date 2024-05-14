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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.EntityId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Details)
                   .IsRequired()
                   .HasMaxLength(5000);

            builder.Property(x => x.CreatedOn)
                    .IsRequired()
                    .HasDefaultValueSql("getutcdate()");
        }
    }
}
