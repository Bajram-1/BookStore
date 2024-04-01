using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.DbConfig
{
    public class ApplicationUsersConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers");

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.StreetAddress).IsRequired(false);
            builder.Property(e => e.City).IsRequired(false);
            builder.Property(e => e.State).IsRequired(false);
            builder.Property(e => e.PostalCode).IsRequired(false);
            builder.Property(e => e.PhoneNumber).IsRequired(false);
            builder.Ignore(e => e.Role);

            builder.HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false); 
        }
    }
}