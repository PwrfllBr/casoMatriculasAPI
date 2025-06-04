using casoMatriculasAPI.Data;
using casoMatriculasAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace casoMatriculasAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        public readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Student>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }
        public async Task<Student?> GetStudentById(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public async void NewStudent(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
        }
        public void DeleteStudent(Student student)
        {
            _context.Students.Remove(student);
        }
    }
}
