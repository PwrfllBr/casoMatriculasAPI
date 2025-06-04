using casoMatriculasAPI.Data;
using casoMatriculasAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace casoMatriculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
        {
            var enrollmentsDto = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new EnrollmentDto
                {
                    IdEnrollment = e.IdEnrollment,
                    IdStudent = e.IdStudent,
                    IdCourse = e.IdCourse,
                    StudentName = $"{e.Student.Name} {e.Student.LastName}",
                    CourseName = e.Course.Name,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate
                }).ToListAsync();
            return Ok(enrollmentsDto); // 200 Ok
        }

        // Get specific enrollment by id
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.IdEnrollment == id);
            if (enrollment == null)
            {
                return NotFound("Enrollment not found"); //404 Not Found
            } else
            {
                var enrollmentDto = new EnrollmentDto
                {
                    IdEnrollment = enrollment.IdEnrollment,
                    IdStudent = enrollment.IdStudent,
                    IdCourse = enrollment.IdCourse,
                    StudentName = $"{enrollment.Student.Name} {enrollment.Student.LastName}",
                    CourseName = enrollment.Course.Name,
                    Status = enrollment.Status,
                    EnrollmentDate = enrollment.EnrollmentDate
                };
                return Ok(enrollmentDto); // 200 Ok
            }
        }

        // Get enrollments by student id
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _context.Enrollments.Where(e => e.IdStudent == studentId)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found for this student"); // 404 Not Found
            }

            var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
                {
                    IdEnrollment = e.IdEnrollment,
                    IdStudent = e.IdStudent,
                    IdCourse = e.IdCourse,
                    StudentName = $"{e.Student.Name} {e.Student.LastName}",
                    CourseName = e.Course.Name,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList();

            return Ok(enrollmentDtos); // 200 Ok
        }

        // Get enrollments by course id
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourseId(int courseId)
        {
            var enrollments = await _context.Enrollments.Where(e => e.IdCourse == courseId)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found for this course"); // 404 Not Found
            }
            
            var enrollmentsDto = enrollments.Select(e => new EnrollmentDto
                {
                    IdEnrollment = e.IdEnrollment,
                    IdStudent = e.IdStudent,
                    IdCourse = e.IdCourse,
                    StudentName = $"{e.Student.Name} {e.Student.LastName}",
                    CourseName = e.Course.Name,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList();
            return Ok(enrollmentsDto); // 200 Ok
        }

        // Get enrollments by status
        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStatus(string status)
        {
            var enrollments = await _context.Enrollments.Where(e => e.Status.ToLower() == status.ToLower())
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found with this status"); // 404 Not Found
            }
            var enrollmentsDto = enrollments.Select(e => new EnrollmentDto
            {
                IdEnrollment = e.IdEnrollment,
                IdStudent = e.IdStudent,
                IdCourse = e.IdCourse,
                StudentName = $"{e.Student.Name} {e.Student.LastName}",
                CourseName = e.Course.Name,
                Status = e.Status,
                EnrollmentDate = e.EnrollmentDate
            }).ToList();

            return Ok(enrollmentsDto); // 200 Ok
        }

        // Create new enrollment
        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> NewEnrollment(NewEnrollmentDto newEnrollmentDto)
        {
            //validate the student and course enrolled exist
            var student = await _context.Students.FindAsync(newEnrollmentDto.IdStudent);
            var course = await _context.Courses.FindAsync(newEnrollmentDto.IdCourse);

            if (newEnrollmentDto == null)
            {
                return BadRequest("Invalid enrollment data");
            }
            if (student == null)
            {
                return NotFound("The student enrolled doesnt exist"); // 404 Not Found
            }
            if (course == null)
            {
                return NotFound("The course enrolled doesnt exist"); // 404 Not Found
            }

            // Validate that student is not enrolled in the course already
            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.IdStudent == newEnrollmentDto.IdStudent && e.IdCourse == newEnrollmentDto.IdCourse);
            if (existingEnrollment)
            {
                return BadRequest("This student is already enrolled in that course"); // 400 Bad Request
            }

            // Validate that date is not in the future
            if (newEnrollmentDto.EnrollmentDate > DateTime.Now)
            {
                return BadRequest("Enrollment date cant be future"); // 400 Bad Request
            }

            // Create new enrollment
            var enrollment = new Models.Enrollment
            {
                IdStudent = newEnrollmentDto.IdStudent,
                IdCourse = newEnrollmentDto.IdCourse,
                EnrollmentDate = newEnrollmentDto.EnrollmentDate,
                Status = "Activa" // Default status for new entries
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            // map dto to send as response
            var enrollmentDto = new EnrollmentDto
            {
                IdEnrollment = enrollment.IdEnrollment,
                IdStudent = enrollment.IdStudent,
                IdCourse = enrollment.IdCourse,
                StudentName = $"{enrollment.Student.Name} {enrollment.Student.LastName}",
                CourseName = enrollment.Course.Name,
                Status = enrollment.Status,
                EnrollmentDate = enrollment.EnrollmentDate
            };

            return StatusCode(StatusCodes.Status201Created, enrollmentDto);
        }

        // Update enrollment status
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateEnrollmentStatus(int id, UpdateEnrollmentDto updateEnrollmentDto)
        {
            // Validate the enrollment exists
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound("Enrollment not found"); // 404 Not Found
            }

            // Validate status
            var validStatuses = new[] { "Activa", "Finalizada", "Cancelada" };
            if (string.IsNullOrEmpty(updateEnrollmentDto.Status) //validate status isnt empty
                || !validStatuses.Contains(updateEnrollmentDto.Status, StringComparer.OrdinalIgnoreCase)) //validate status is one of the valid options
            {
                return BadRequest("Invalid status. Must be 'Activa', 'Finalizada', or 'Cancelada'.");
            }

            // Validate that the enrollment is not in the updated status already
            if (enrollment.Status.Equals(updateEnrollmentDto.Status, StringComparison.OrdinalIgnoreCase)) //using ordinalignorecase comparison to bypass cases
            {
                return BadRequest("Enrollment is already in the updated status");
            }

            // validate status is not "Finalizada" if updated status is "Cancelada"
            if (enrollment.Status.Equals("Finalizada", StringComparison.OrdinalIgnoreCase) &&
                updateEnrollmentDto.Status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot cancel an enrollment if its already finalized.");
            }

            // Update the enrollment status
            enrollment.Status = updateEnrollmentDto.Status;
            await _context.SaveChangesAsync();
            return Ok("Enrollment status updated successfully"); // 200 Ok
        }

        // Delete enrollment
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            // Validate the enrollment exists
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound("Enrollment not found"); // 404 Not Found
            }

            // Check if the enrollment is already 'Cancelada'
            if (!enrollment.Status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cant delete an enrollment unless its canceled");
            }

            // Delete the enrollment
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return Ok("Enrollment deleted successfully"); // 200 Ok
        }
    }
}
