using casoMatriculasAPI.Models;

namespace casoMatriculasAPI.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetStudents();
        Task<Student?> GetStudentById(int id);
        void NewStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(Student student);
    }
}
