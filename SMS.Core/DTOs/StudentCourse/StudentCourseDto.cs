using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.StudentCourse
{
    public class StudentCourseDto
    {
        public int CourseId { get; set; }
        public string? Title { get; set; } 
        public string? Description { get; set; } 
        public double Grade { get; set; }
    }

}
