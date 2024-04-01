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
    public class CompaniesConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.StreetAddress).IsRequired(false);
            builder.Property(e => e.City).IsRequired(false);
            builder.Property(e => e.State).IsRequired(false);
            builder.Property(e => e.PostalCode).IsRequired(false);
            builder.Property(e => e.PhoneNumber).IsRequired(false);
        }
    }
}