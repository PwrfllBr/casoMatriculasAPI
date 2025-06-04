using casoMatriculasAPI.Models;

namespace casoMatriculasAPI.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCourses();
        Task<Course?> GetCourseById(int id);
        void NewCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
    }
}
