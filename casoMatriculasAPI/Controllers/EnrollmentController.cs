using casoMatriculasAPI.Data;
using casoMatriculasAPI.DTOs;
using casoMatriculasAPI.Models;
using casoMatriculasAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace casoMatriculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(ICourseRepository courseRepository, IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository, ApplicationDbContext context,
            ILogger<EnrollmentController> logger)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _context = context;
            _logger = logger;
        }

        // Get all enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
        {

            _logger.LogInformation("getting all enrollments");
            var enrollments = await _enrollmentRepository.GetEnrollments();

            var enrollmentsDto = enrollments
                .Select(e => new EnrollmentDto
                {
                    IdEnrollment = e.IdEnrollment,
                    IdStudent = e.IdStudent,
                    IdCourse = e.IdCourse,
                    StudentName = $"{e.Student.Name} {e.Student.LastName}",
                    CourseName = e.Course.Name,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList();

            _logger.LogInformation($"Successfully fetched {enrollmentsDto.Count} enrollments");
            return Ok(enrollmentsDto); // 200 Ok
        }

        // Get specific enrollment by id
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(int id)
        {
            _logger.LogInformation($"Getting enrollment with id: {id}");
            var enrollment = await _enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                _logger.LogInformation($"Enrollment with id: {id} not found");
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
                _logger.LogInformation($"Successfully fetched enrollment with id: {id}");
                return Ok(enrollmentDto); // 200 Ok
            }
        }

        // Get enrollments by student id
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStudentId(int studentId)
        {
            _logger.LogInformation($"Getting enrollments from student with id: {studentId}");
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentId(studentId);
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                _logger.LogInformation($"No enrollments found from student with id: {studentId}");
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
            _logger.LogInformation($"Successfully fetched {enrollmentDtos.Count} enrollments");
            return Ok(enrollmentDtos); // 200 Ok
        }

        // Get enrollments by course id
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourseId(int courseId)
        {
            _logger.LogInformation($"Getting enrollments from course with id: {courseId}");
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseId(courseId);
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                _logger.LogInformation($"No enrollments found from course with id: {courseId}");
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
            _logger.LogInformation($"Successfully fetched {enrollmentsDto.Count} enrollments");
            return Ok(enrollmentsDto); // 200 Ok
        }

        // Get enrollments by status
        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStatus(string status)
        {
            _logger.LogInformation($"Getting enrollments with status: {status}");
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStatus(status);
            //if no enrollments found, return 404
            if (enrollments == null || !enrollments.Any())
            {
                _logger.LogInformation($"No enrollments found with status: {status}");
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
            _logger.LogInformation($"Successfully fetched {enrollmentsDto.Count} enrollments");

            return Ok(enrollmentsDto); // 200 Ok
        }

        // Create new enrollment
        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> NewEnrollment(NewEnrollmentDto newEnrollmentDto)
        {
            _logger.LogInformation("Attempting to create new enrollment");
            //validate the student and course enrolled exist
            var student = await _studentRepository.GetStudentById(newEnrollmentDto.IdStudent);
            var course = await _courseRepository.GetCourseById(newEnrollmentDto.IdCourse);

            if (newEnrollmentDto == null)
            {
                _logger.LogInformation("New enrollment request had missing data");
                return BadRequest("Invalid enrollment data");
            }
            if (student == null)
            {
                _logger.LogInformation("New enrollment request had not existing student");
                return NotFound("The student enrolled doesnt exist"); // 404 Not Found
            }
            if (course == null)
            {
                _logger.LogInformation("New enrollment request had not existing course");
                return NotFound("The course enrolled doesnt exist"); // 404 Not Found
            }

            // Validate that student is not enrolled in the course already
            var existingEnrollment = await _enrollmentRepository.checkDuplicates(newEnrollmentDto.IdStudent, newEnrollmentDto.IdCourse);
            if (existingEnrollment)
            {
                _logger.LogInformation("New enrollment request had already been registered");
                return BadRequest("This student is already enrolled in that course"); // 400 Bad Request
            }

            // Validate that date is not in the future
            if (newEnrollmentDto.EnrollmentDate > DateTime.Now)
            {
                _logger.LogInformation("New enrollment request had future date");
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

            _enrollmentRepository.NewEnrollment(enrollment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("New enrollment successfully created");
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
            _logger.LogInformation($"Attempting to update enrollment with id: {id}");
            //params for stored procedure
            var spParameters = new[]
            {
                new SqlParameter("@EnrollmentId", id),
                new SqlParameter("@UpdatedStatus", updateEnrollmentDto.Status)
            };

            try
            {
                //execute stored procedure to update status
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_updateEnrollmentStatus @EnrollmentId, @UpdatedStatus",
                    spParameters);

                _logger.LogInformation($"Enrollment with id: {id} successfully updated");
                return Ok("Enrollment status updated successfully"); // 200 Ok
            }

            catch (SqlException ex)
            {
                if (ex.Number == 50000)
                {
                    _logger.LogError($"Error updating enrollment with id: {id} - {ex.Message}");
                    return BadRequest(ex.Message); // error message set in stored procedure
                }
                // if error is not handled:
                _logger.LogError($"Unexpected error updating enrollment with id: {id} - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the enrollment status"); // 500 Internal Server Error
            }
        }

        // Delete enrollment
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            _logger.LogInformation($"Attempting to delete enrollment with id: {id}");
            // Validate the enrollment exists
            var enrollment = await _enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                _logger.LogInformation($"Enrollment with id: {id} not found");
                return NotFound("Enrollment not found"); // 404 Not Found
            }

            // Check if the enrollment is already 'Cancelada'
            if (!enrollment.Status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cant delete an enrollment unless its canceled");
            }

            // Delete the enrollment
            _enrollmentRepository.DeleteEnrollment(enrollment);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Enrollment with id: {id} deleted successfully");
            return Ok("Enrollment deleted successfully"); // 200 Ok
        }
    }
}
