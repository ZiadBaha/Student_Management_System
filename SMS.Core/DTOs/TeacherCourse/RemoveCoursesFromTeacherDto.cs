using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.TeacherCourse
{
    public class RemoveCoursesFromTeacherDto
    {
        public int TeacherId { get; set; }
        public List<int> CourseIds { get; set; } = new();
    }
}
