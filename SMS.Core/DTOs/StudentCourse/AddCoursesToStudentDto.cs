using SMS.Core.DTOs.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.StudentCourse
{
    public class AddCoursesToStudentDto
    {
        public string? StudentId { get; set; }
        public List<CourseGradeDto> Courses { get; set; } = new();
    }
}
