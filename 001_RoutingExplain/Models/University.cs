namespace _001_RoutingExplain.Models
{
    public class University
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public University()
        {
            Teachers = new List<Teacher>();
            Students = new List<Student>();
        }
        public List<Teacher> Teachers { get; set; }
        public List<Student> Students { get; set; }
    }
    public class Teacher
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public University? University { get; set; }
    }

    public class Student
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public University? University { get; set; }
    }
}


