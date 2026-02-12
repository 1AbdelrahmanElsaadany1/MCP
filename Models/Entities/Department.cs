namespace MCPv1.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
