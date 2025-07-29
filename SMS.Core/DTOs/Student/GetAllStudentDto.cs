using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.DTOs.Student
{
    public class GetAllStudentDto
    {
        public string? Id { get; set; } 
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public string? Email { get; set; } 
        public DateTime DateOfBirth { get; set; }
    }

}
