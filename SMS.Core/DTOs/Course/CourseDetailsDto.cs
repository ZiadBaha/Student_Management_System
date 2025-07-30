using SMS.Core.DTOs.Enrollment;
using SMS.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.Course
{
    public class CourseDetailsDto : CourseDto
    {
         

        public List<EnrolledStudentDto> EnrolledStudents { get; set; } = new();
        public List<TeacherInfoDto> Teachers { get; set; } = new();
    }
}
