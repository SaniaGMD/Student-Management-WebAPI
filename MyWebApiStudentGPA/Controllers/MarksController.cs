using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DL.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarksController : ControllerBase
    {
        private readonly StudentDbContext _dbContext;

        public MarksController(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST api/marks
        [HttpPost]
        public IActionResult RecordMarks([FromBody] StudentSubjectDbDto studentSubjectMarks)
        {
            if (studentSubjectMarks == null || studentSubjectMarks.SID <= 0 || studentSubjectMarks.SubjectId <= 0 || studentSubjectMarks.GPA <= 0 || studentSubjectMarks.Marks < 0)
            {
                return BadRequest("Invalid marks data");
            }

            var student = _dbContext.Students.Find(studentSubjectMarks.SID);
            var subject = _dbContext.Subjects.Find(studentSubjectMarks.SubjectId);

            if (student == null || subject == null)
            {
                return NotFound("Student or subject not found");
            }

            var existingMarks = _dbContext.StudentSubjects
                .FirstOrDefault(ss => ss.SID == studentSubjectMarks.SID && ss.SubjectId == studentSubjectMarks.SubjectId);

            if (existingMarks == null)
            {
                // If marks entry doesn't exist, create a new one
                existingMarks = new StudentSubjectDbDto
                {
                    SID = studentSubjectMarks.SID,
                    SubjectId = studentSubjectMarks.SubjectId,
                    GPA = studentSubjectMarks.GPA,
                    Marks = studentSubjectMarks.Marks
                };

                _dbContext.StudentSubjects.Add(existingMarks);
            }
            else
            {
                // Update existing marks
                existingMarks.GPA = studentSubjectMarks.GPA;
                existingMarks.Marks = studentSubjectMarks.Marks;
            }

            _dbContext.SaveChanges();

            return Ok(existingMarks);
        }

        // PUT api/marks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateMarks(int id, [FromBody] StudentSubjectDbDto updateMarks)
        {
            if (updateMarks == null || id <= 0 || updateMarks.Marks < 0)
            {
                return BadRequest("Invalid marks data");
            }

            var existingMarks = _dbContext.StudentSubjects.Find(id);

            if (existingMarks == null)
            {
                return NotFound("Marks not found");
            }

            existingMarks.GPA = updateMarks.GPA;
            existingMarks.Marks = updateMarks.Marks;

            _dbContext.SaveChanges();

            return Ok();
        }

        // DELETE api/marks/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteMarks(int id)
        {
            var marksToRemove = _dbContext.StudentSubjects.Find(id);

            if (marksToRemove == null)
            {
                return NotFound("Marks not found");
            }

            _dbContext.StudentSubjects.Remove(marksToRemove);
            _dbContext.SaveChanges();

            return Ok();
        }
        // GET api/students/{student_id}/subjects/{subject_id}/marks
        [HttpGet("{studentId}/subjects/{subjectId}/marks")]
        public IActionResult GetMarksForSubject(int studentId, int subjectId)
        {
            var student = _dbContext.Students.Find(studentId);
            var subject = _dbContext.Subjects.Find(subjectId);

            if (student == null || subject == null)
            {
                return NotFound("Student or subject not found");
            }

            var marks = _dbContext.StudentSubjects
                .Where(ss => ss.SID == studentId && ss.SubjectId == subjectId)
                .Select(ss => new
                {
                    MarksId = ss.Id,
                    GPA = ss.GPA,
                    Marks = ss.Marks
                })
                .FirstOrDefault();

            if (marks == null)
            {
                return NotFound("Marks not found for the specified subject and student");
            }

            return Ok(marks);
        }

        // GET api/students/{student_id}/marks
        [HttpGet("{studentId}/marks")]
        public IActionResult GetAllMarksForStudent(int studentId)
        {
            var student = _dbContext.Students.Find(studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var allMarks = _dbContext.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .Select(ss => new
                {
                    SubjectId = ss.SubjectId,
                    SubjectName = ss.SubjectDbDto.Name,
                    GPA = ss.GPA,
                    Marks = ss.Marks
                })
                .ToList();

            if (!allMarks.Any())
            {
                return NotFound("No marks found for the specified student");
            }

            return Ok(allMarks);
        }
    }
}
