using casoMatriculasAPI.Data;
using casoMatriculasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace casoMatriculasAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public void NewCourse(Course course)
        {
            _context.Courses.Add(course);
        }

        public void UpdateCourse(Course course)
        {
            _context.Courses.Update(course);
        }
    }
}
