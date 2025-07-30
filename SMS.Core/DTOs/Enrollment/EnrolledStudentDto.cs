using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.Enrollment
{
    public class EnrolledStudentDto
    {
        public string? StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public double Grade { get; set; }
    }
}
