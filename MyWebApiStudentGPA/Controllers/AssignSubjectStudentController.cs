using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DL.DbModels; 

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignSubjectStudentController : ControllerBase
    {
        private readonly StudentDbContext _dbContext;

        public AssignSubjectStudentController(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST api/assign-subject-student
        [HttpPost("student-subjects")]
        public IActionResult AssignSubjectToStudent([FromBody] AssignmentDto assignment)
        {
            if (assignment == null || assignment.StudentId <= 0 || assignment.SubjectId <= 0)
            {
                return BadRequest("Invalid assignment data");
            }

            var student = _dbContext.Students.Find(assignment.StudentId);
            var subject = _dbContext.Subjects.Find(assignment.SubjectId);

            if (student == null || subject == null)
            {
                return NotFound("Student or subject not found");
            }

            var assignmentEntity = new StudentSubjectDbDto
            {
                studentDbDto = student,
                SubjectDbDto = subject
            };

            _dbContext.StudentSubjects.Add(assignmentEntity);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetAssignment), new { id = assignmentEntity.Id }, assignmentEntity);
        }

        // PUT api/assign-subject-student/student-subjects/{assignment_id}
        [HttpPut("student-subjects/{id}")]
        public IActionResult UpdateAssignment(int id, [FromBody] AssignmentDto updatedAssignment)
        {
            if (updatedAssignment == null || id <= 0)
            {
                return BadRequest("Invalid assignment data");
            }

            var existingAssignment = _dbContext.StudentSubjects.Find(id);

            if (existingAssignment == null)
            {
                return NotFound("Assignment not found");
            }

            // Update assignment details if needed

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("student-subjects/{id}")]
        public IActionResult GetAssignment(int id)
        {
            var assignment = _dbContext.StudentSubjects.Find(id);

            if (assignment == null)
            {
                return NotFound("Assignment not found");
            }

            return Ok(assignment);
        }
        // DELETE api/assign-subject-student/student-subjects/{assignment_id}
        [HttpDelete("student-subjects/{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            var assignmentToRemove = _dbContext.StudentSubjects.Find(id);

            if (assignmentToRemove == null)
            {
                return NotFound("Assignment not found");
            }

            _dbContext.StudentSubjects.Remove(assignmentToRemove);
            _dbContext.SaveChanges();

            return Ok();
        }

        // GET api/students/{student_id}/subjects
        [HttpGet("{studentId}/subjects")]
        public IActionResult GetStudentSubjects(int studentId)
        {
            var student = _dbContext.Students.Find(studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var studentSubjects = _dbContext.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .Select(ss => new
                {
                    SubjectId = ss.SubjectId,
                    SubjectName = ss.SubjectDbDto.Name,
                    GPA = ss.GPA,
                    Marks = ss.Marks
                })
                .ToList();

            return Ok(studentSubjects);
        }


        public class AssignmentDto
        {
            public int StudentId { get; set; }
            public int SubjectId { get; set; }
        }
    }

}
