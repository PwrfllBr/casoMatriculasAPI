using casoMatriculasAPI.Models;

namespace casoMatriculasAPI.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetEnrollments();
        Task<Enrollment?> GetEnrollmentById(int id);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int id);

        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseId(int id);

        Task<IEnumerable<Enrollment>> GetEnrollmentsByStatus(string status);

        void NewEnrollment(Enrollment enrollment);
        void UpdateEnrollment(Enrollment enrollment);
        void DeleteEnrollment(Enrollment enrollment);

        Task<bool> checkDuplicates(int studentId, int courseId);
    }
}
