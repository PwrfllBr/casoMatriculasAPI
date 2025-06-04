namespace casoMatriculasAPI.DTOs
{
    public class CourseDto
    {
        public int IdCourse { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class NewCourseDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
