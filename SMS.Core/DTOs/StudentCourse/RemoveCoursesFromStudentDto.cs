using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.StudentCourse
{
    public class RemoveCoursesFromStudentDto
    {
        public string? StudentId { get; set; }
        public List<int> CourseIds { get; set; } = new();
    }

}
