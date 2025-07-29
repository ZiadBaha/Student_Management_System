namespace SMS.Core.Models.Entities
{
    public class CourseTeacher : BaseEntity
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}