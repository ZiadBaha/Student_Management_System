using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SMS.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Data.Config
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(s => s.AppUser)
                   .WithOne()
                   .HasForeignKey<Student>(s => s.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.DateOfBirth)
                   .IsRequired();
        }
    }
}
