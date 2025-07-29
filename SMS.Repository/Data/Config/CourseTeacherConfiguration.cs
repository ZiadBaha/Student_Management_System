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
    public class CourseTeacherConfiguration : IEntityTypeConfiguration<CourseTeacher>
    {
        public void Configure(EntityTypeBuilder<CourseTeacher> builder)
        {
            builder.HasOne(ct => ct.Course)
                   .WithMany(c => c.CourseTeachers)
                   .HasForeignKey(ct => ct.CourseId);

            builder.HasOne(ct => ct.Teacher)
                   .WithMany(t => t.CourseTeachers)
                   .HasForeignKey(ct => ct.TeacherId);
        }
    }
}
