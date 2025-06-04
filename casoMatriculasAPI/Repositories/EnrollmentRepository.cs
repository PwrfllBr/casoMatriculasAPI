using casoMatriculasAPI.Data;
using casoMatriculasAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace casoMatriculasAPI.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> checkDuplicates(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.IdStudent == studentId && e.IdCourse == courseId);
        }

        public void DeleteEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
        }

        public async Task<Enrollment?> GetEnrollmentById(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.IdEnrollment == id);
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollments()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseId(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.IdCourse == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStatus(string status)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Status == status.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.IdStudent == id)
                .ToListAsync();
        }

        public async void NewEnrollment(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }

        public void UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
        }
    }
}
