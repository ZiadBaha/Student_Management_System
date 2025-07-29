using SMS.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Entities
{
    public class Student : BaseEntity
    {
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
