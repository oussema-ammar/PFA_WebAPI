using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFA_WebAPI.DTO;
using PFA_WebAPI.Interfaces;
using PFA_WebAPI.Models;
using System.Security.Claims;

namespace PFA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly IDocumentRepository _documentRepository;
        public JobController(IJobRepository jobRepository, IDocumentRepository documentRepository)
        {
            _jobRepository = jobRepository;
            _documentRepository = documentRepository;
        }

        [HttpGet, Authorize]
        public int GetMe()
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        // GET: api/<JobController>
        [HttpGet("User/{UserId}"), Authorize]
        public IActionResult GetJobs(int UserId)
        {
            var jobs = _jobRepository.GetJobs(UserId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(jobs);
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetJob(int id)
        {
            var job = _jobRepository.GetJob(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(job);
        }

        // POST api/<JobController>
        [HttpPost("AddJob"), Authorize(Roles = "Client")]
        public IActionResult CreateJob(JobCreateDTO request)
        {
            var job = new Job
            {
                Name = request.Name,
                Description = request.Description,
                Client_Id = GetMe(),
                Engineer_Id = request.Engineer_Id
            };
            _jobRepository.Addjob(job);
            return Ok(job.Id);
        }


        // PUT api/<JobController>/5
        [HttpPut("{id}/edit"), Authorize(Roles = "Admin,Client")]
        public IActionResult Put(int id, JobUpdateDTO jobupdateDTO)
        {
            try
            {
                // Retrieve the job from the repository
                var job = _jobRepository.GetJob(id);
                // Update the job's properties with the new values
                job.Name = jobupdateDTO.Name;
                job.Description = jobupdateDTO.Description;
                job.Status = jobupdateDTO.Status;
                job.Deadline = jobupdateDTO.Deadline;
                // Save the changes to the database
                _jobRepository.UpdateJob(job);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/Accept"), Authorize(Roles = "Engineer")]
        public IActionResult AcceptJob(int id)
        {
            try
            {
                var job = _jobRepository.GetJob(id);
                job.Status = "Accepted";
                _jobRepository.UpdateJob(job);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}/Submit"), Authorize(Roles = "Engineer")]
        public IActionResult SubmitJob(int id)
        {
            try
            {
                var job = _jobRepository.GetJob(id);
                job.Status = "Submitted";
                _jobRepository.UpdateJob(job);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/Complete"), Authorize(Roles = "Client")]
        public IActionResult CompleteJob(int id)
        {
            try
            {
                var job = _jobRepository.GetJob(id);
                job.Status = "Complete";
                _jobRepository.UpdateJob(job);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<JobController>/5
        [HttpDelete("{id}"), Authorize(Roles ="Admin,Client")]
        public IActionResult Delete(int id)
        {
            try
            {
                _jobRepository.DeleteJob(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("Search"),Authorize]
        public IActionResult SearchJobs(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }
            var jobs = _jobRepository.SearchJobs(query);
            return Ok(jobs);
        }

        [HttpPost("{jobId}/documents"), Authorize]
        public IActionResult UploadDocument(int jobId, IFormFile file)
        {
            // Check if the job exists
            var job = _jobRepository.GetJob(jobId);
            if (job == null)
            {
                return BadRequest("Job doesn't exist");
            }
            // Check if a file was uploaded
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }
            // Generate a unique file name
            var fileName = $"{Guid.NewGuid()}-{file.FileName}";
            // Save the file to the server
            var filePath = Path.Combine("uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            // Create a new document object
            var document = new Document
            {
                Name = fileName,
                Path = filePath,
                JobId= jobId,
            };
            // Add the document to the job
            job.Documents.Add(document);
            _jobRepository.UpdateJob(job);

            return Ok(document);
        }
        [HttpGet("download/{documentId}"), Authorize]
        public IActionResult DownloadDocument(int documentId)
        {
            var document = _documentRepository.GetDocument(documentId);

            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.Path);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/octet-stream", document.Name);
        }

        [HttpDelete("DeleteDocument/{documentId}"), Authorize]
        public IActionResult DeleteDocument(int documentId)
        {
            try
            {
                var document = _documentRepository.GetDocument(documentId);
                if (document == null)
                {
                    return NotFound();
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.Path);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _documentRepository.DeleteDocument(documentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
    }
}