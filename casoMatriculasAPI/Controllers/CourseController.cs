using casoMatriculasAPI.Data;
using casoMatriculasAPI.DTOs;
using casoMatriculasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace casoMatriculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        //CRUD

        // Get all courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var coursesDto = _context.Courses.Select(c => new CourseDto
            {
                IdCourse = c.IdCourse,
                Name = c.Name,
                Description = c.Description
            }).ToList();
            return Ok(coursesDto); // 200 OK
        }

        // Get specific course
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCoursebyId(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
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
                return Ok(courseDto); // 200 OK
            }
        }

        // Create new course
        [HttpPost]
        public async Task<ActionResult<CourseDto>> NewCourse(NewCourseDto newCourseDto)
        {
            if (newCourseDto == null)
            {
                return BadRequest("Missing course data"); // 400 Bad Request
            }
            var course = new Course
            {
                Name = newCourseDto.Name,
                Description = newCourseDto.Description
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
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
            var updatedCourse = await _context.Courses.FindAsync(id);
            if (updatedCourse == null)
            {
                return NotFound("Course not found"); // 404 Not Found
            }
            if (updatedCourseDto == null)
            {
                return BadRequest("Missing course data"); // 400 Bad Request
            }
            else
            {
                updatedCourse.Name = updatedCourseDto.Name;
                updatedCourse.Description = updatedCourseDto.Description;

                await _context.SaveChangesAsync();
                return Ok("Course updated"); // 200 ok  
            }
        }

        // Delete course
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Course not found"); // 404 Not Found
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok("Course deleted successfully"); // 200 OK
        }
    }
}
