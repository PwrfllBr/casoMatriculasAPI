using Microsoft.AspNetCore.Mvc;
using casoMatriculasAPI.Data;
using casoMatriculasAPI.Models;
using Microsoft.EntityFrameworkCore;
using casoMatriculasAPI.DTOs;
using casoMatriculasAPI.Repositories;

namespace casoMatriculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;

        public StudentController(IStudentRepository studentRepository, ApplicationDbContext context)
        {
            _studentRepository = studentRepository;
            _context = context;
        }

        //CRUD

        //Get all students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _studentRepository.GetStudents();
            var studentsDto = students.Select(s => new StudentDto
            {
                IdStudent = s.IdStudent,
                Name = s.Name,
                LastName = s.LastName,
                Email = s.Email
            }).ToList();
            return Ok(studentsDto); //200 OK
        }

        //Get specific student
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentbyId(int id)
        {
            var student = await _studentRepository.GetStudentById(id);
            
            if (student == null)
            {
                return NotFound("Student not found"); //404 Not Found
            }
            else
            {
                var studentDto = new StudentDto
                {
                    IdStudent = student.IdStudent,
                    Name = student.Name,
                    LastName = student.LastName,
                    Email = student.Email,
                };
                return Ok(studentDto); //200 OK
            }
        }

        // Create new student
        [HttpPost]
        public async Task<ActionResult<StudentDto>> NewStudent(NewStudentDto newStudentDto)
        {
            if (newStudentDto == null)
            {
                return BadRequest("Missing student data"); //400 Bad Request
            } else {
                var student = new Student
                {
                    Name = newStudentDto.Name,
                    LastName = newStudentDto.LastName,
                    Email = newStudentDto.Email
                };
                _studentRepository.NewStudent(student);
                await _context.SaveChangesAsync();

                // Return the created student as a DTO
                var studentDto = new StudentDto
                {
                    IdStudent = student.IdStudent,
                    Name = student.Name,
                    LastName = student.LastName,
                    Email = student.Email
                };

                return StatusCode(StatusCodes.Status201Created, studentDto); //201 Created
            }
        }

        // Update student
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateStudent(int id, NewStudentDto updatedStudentDto)
        {
            var updatedStudent = await _studentRepository.GetStudentById(id);
            if (updatedStudent == null)
            {
                return NotFound("Student not found"); //404 Not Found
            }

            if (updatedStudentDto == null)
            {
                return BadRequest("Missing student data"); //400 Bad Request
            }
            else
            {
                updatedStudent.Name = updatedStudentDto.Name;
                updatedStudent.LastName = updatedStudentDto.LastName;
                updatedStudent.Email = updatedStudentDto.Email;

                _studentRepository.UpdateStudent(updatedStudent);
                await _context.SaveChangesAsync();
                return Ok("Student updated"); //200 Ok
            }
        }

        // Delete student
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetStudentById(id);
            if (student == null)
            {
                return NotFound("Student not found"); //404 Not Found
            }

            _studentRepository.DeleteStudent(student);
            await _context.SaveChangesAsync();

            return Ok("Student deleted successfully");
        }

    }
}
