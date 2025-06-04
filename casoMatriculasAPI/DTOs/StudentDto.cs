namespace casoMatriculasAPI.DTOs
{
    public class StudentDto
    {
        public int IdStudent { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
    }
    public class NewStudentDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
    }
}
