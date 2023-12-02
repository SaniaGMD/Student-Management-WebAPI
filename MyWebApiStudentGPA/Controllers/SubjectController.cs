using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DL.DbModels; 

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly StudentDbContext _dbContext;

        public SubjectController(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/subject
        [HttpGet]
        public IActionResult Get()
        {
            var subjects = _dbContext.Subjects.ToList();
            return Ok(subjects);
        }

        // POST api/subject
        [HttpPost]
        public IActionResult Post([FromBody] SubjectDbDto subject)
        {
            if (subject == null)
            {
                return BadRequest("Invalid subject data");
            }

            _dbContext.Subjects.Add(subject);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = subject.id }, subject);
        }

        // PUT api/subject/{subject_id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SubjectDbDto updatedSubject)
        {
            if (updatedSubject == null)
            {
                return BadRequest("Invalid subject data");
            }

            var existingSubject = _dbContext.Subjects.FirstOrDefault(s => s.id == id);

            if (existingSubject == null)
            {
                return NotFound();
            }

            existingSubject.Name = updatedSubject.Name;

            _dbContext.SaveChanges();

            return Ok();
        }

        // DELETE api/subject/{subject_id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subjectToRemove = _dbContext.Subjects.FirstOrDefault(s => s.id == id);

            if (subjectToRemove == null)
            {
                return NotFound();
            }

            _dbContext.Subjects.Remove(subjectToRemove);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetSubject(int id)
        {
            var subject = _dbContext.Subjects.Find(id);

            if (subject == null)
            {
                return NotFound("Subject not found");
            }

            return Ok(subject);
        }
    }
}
