using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DL.DbModels;

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _dbContext;

        public StudentController(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

     

        // GET api/student
        [HttpGet]
        public IActionResult Get()
        {
            var students = _dbContext.Students.ToList();
            return Ok(students);
        }

        // POST api/student
        [HttpPost]
        public IActionResult Post([FromBody] StudentDbDto student)
        {
            if (student == null)
            {
                return BadRequest("Invalid student data");
            }

            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }

        // PUT api/student/{student_id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] StudentDbDto updatedStudent)
        {
            if (updatedStudent == null)
            {
                return BadRequest("Invalid student data");
            }

            var existingStudent = _dbContext.Students.FirstOrDefault(s => s.Id == id);

            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.Name = updatedStudent.Name;
            existingStudent.RollNumber = updatedStudent.RollNumber;
            existingStudent.PhoneNumber = updatedStudent.PhoneNumber;

            _dbContext.SaveChanges();

            return Ok();
        }

        // DELETE api/student/{student_id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var studentToRemove = _dbContext.Students.FirstOrDefault(s => s.Id == id);

            if (studentToRemove == null)
            {
                return NotFound();
            }

            _dbContext.Students.Remove(studentToRemove);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            var student = _dbContext.Students.Find(id);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            return Ok(student);
        }

        // GET api/students/{student_id}/gpa
        [HttpGet("{studentId}/gpa")]
        public IActionResult GetStudentGPA(int studentId)
        {
            var student = _dbContext.Students.Find(studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var studentSubjects = _dbContext.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .ToList();

            if (!studentSubjects.Any())
            {
                return NotFound("No subjects found for the specified student");
            }

            var totalGPA = studentSubjects.Sum(ss => ss.GPA);
            var currentGPA = totalGPA / studentSubjects.Count;

            return Ok(new { StudentId = studentId, GPA = currentGPA });
        }
    }
}
