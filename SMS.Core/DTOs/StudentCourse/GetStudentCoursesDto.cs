using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.StudentCourse
{
    public class GetStudentCoursesDto
    {
        public string? StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<StudentCourseDto> Courses { get; set; } = new();
    }

}
