using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.TeacherCourse
{
    public class UpdateTeacherCoursesDto
    {
        public string? TeacherId { get; set; }
        public List<int> CourseIds { get; set; } = new();
    }
}
