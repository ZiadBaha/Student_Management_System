namespace SMS.Core.Models.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}