using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.StudentCourse
{
    public class UpdateStudentCoursesDto
    {
        public string? StudentId { get; set; }
        public List<UpdateCourseGradeDto> Courses { get; set; } = new();
    }
}
