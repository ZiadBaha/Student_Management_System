using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.TeacherCourse
{
    public class GetTeacherCoursesDto
    {
        public string? TeacherId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<TeacherCourseDto> Courses { get; set; } = new();
    }
}
