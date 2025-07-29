using SMS.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Entities
{
    public class Teacher : BaseEntity
    {
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();
    }
}
