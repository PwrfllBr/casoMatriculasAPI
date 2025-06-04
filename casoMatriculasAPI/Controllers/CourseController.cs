using casoMatriculasAPI.Data;
using casoMatriculasAPI.DTOs;
using casoMatriculasAPI.Models;
using casoMatriculasAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace casoMatriculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseController> _logger;

        public CourseController( ICourseRepository courseRepository,
            ApplicationDbContext context,
            ILogger<CourseController> logger)
        {
            _courseRepository = courseRepository;
            _context = context;
            _logger = logger;
        }

        //CRUD

        // Get all courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            _logger.LogInformation("Getting all courses");
            var courses = await _courseRepository.GetCourses();
            var coursesDto = courses.Select(c => new CourseDto
            {
                IdCourse = c.IdCourse,
                Name = c.Name,
                Description = c.Description
            }).ToList();
            _logger.LogInformation($"Successfully fetched {coursesDto.Count} courses");
            return Ok(coursesDto); // 200 OK
        }

        // Get specific course
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCoursebyId(int id)
        {
            _logger.LogInformation($"Getting course with id: {id}");
            var course = await _courseRepository.GetCourseById(id);

            if (course == null)
            {
                _logger.LogInformation($"Course with id: {id} not found");
                return NotFound("Course not found"); // 404 Not Found
            }
            else
            {
                var courseDto = new CourseDto
                {
                    IdCourse = course.IdCourse,
                    Name = course.Name,
                    Description = course.Description,
                };
                _logger.LogInformation($"Successfully fetched course with id: {id}");
                return Ok(courseDto); // 200 OK
            }
        }

        // Create new course
        [HttpPost]
        public async Task<ActionResult<CourseDto>> NewCourse(NewCourseDto newCourseDto)
        {
            _logger.LogInformation("Attempting to create new course");
            if (newCourseDto == null)
            {
                _logger.LogInformation("New course request had missing data");
                return BadRequest("Missing course data"); // 400 Bad Request
            }
            var course = new Course
            {
                Name = newCourseDto.Name,
                Description = newCourseDto.Description
            };
            _courseRepository.NewCourse(course);
            await _context.SaveChangesAsync();
            _logger.LogInformation("New course successfully created");
            // Return the created course as a DTO
            var courseDto = new CourseDto
            {
                IdCourse = course.IdCourse,
                Name = course.Name,
                Description = course.Description
            };
            return StatusCode(StatusCodes.Status201Created, courseDto); // 201 Created
        }

        // update course
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateCourse(int id, NewCourseDto updatedCourseDto)
        {
            _logger.LogInformation($"Attempting to update course with id: {id}");
            var updatedCourse = await _courseRepository.GetCourseById(id);
            if (updatedCourse == null)
            {
                _logger.LogInformation($"Course with id: {id} not found");
                return NotFound("Course not found"); // 404 Not Found
            }
            if (updatedCourseDto == null)
            {
                _logger.LogInformation("Course update request had missing data");
                return BadRequest("Missing course data"); // 400 Bad Request
            }
            else
            {
                updatedCourse.Name = updatedCourseDto.Name;
                updatedCourse.Description = updatedCourseDto.Description;

                _courseRepository.UpdateCourse(updatedCourse);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Course with id: {id} updated successfully");
                return Ok("Course updated"); // 200 ok  
            }
        }

        // Delete course
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            _logger.LogInformation($"Attempting to delete course with id: {id}");
            var course = await _courseRepository.GetCourseById(id);
            if (course == null)
            {
                _logger.LogInformation($"Course with id: {id} not found");
                return NotFound("Course not found"); // 404 Not Found
            }
            _courseRepository.DeleteCourse(course);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Course with id: {id} deleted successfully");
            return Ok("Course deleted successfully"); // 200 OK
        }
    }
}
