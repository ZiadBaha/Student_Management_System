namespace SMS.Core.Models.Entities
{
    public class Enrollment : BaseEntity
    {

        public string? StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public double Grade { get; set; }
    }
}