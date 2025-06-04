using System.ComponentModel.DataAnnotations;

namespace casoMatriculasAPI.DTOs
{
    public class EnrollmentDto
    {
        public int IdEnrollment { get; set; }
        public int IdStudent { get; set; }
        public int IdCourse { get; set; }
        public string StudentName { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
    }
    public class NewEnrollmentDto
    {
        public int IdStudent { get; set; }
        public int IdCourse { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
    public class UpdateEnrollmentDto
    {
        public string Status { get; set; } = null!;
    }
}
