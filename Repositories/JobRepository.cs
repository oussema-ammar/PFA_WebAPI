using Microsoft.EntityFrameworkCore;
using PFA_WebAPI.Data;
using PFA_WebAPI.DTO;
using PFA_WebAPI.Interfaces;
using PFA_WebAPI.Models;
using System;

namespace PFA_WebAPI.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly DataContext _context;
        public JobRepository(DataContext context)
        {
            _context = context;
        }

        public bool Addjob(Job job)
        {
            _context.Jobs.Add(job);
            _context.SaveChanges();
            return true;
        }

        public void DeleteJob(int id)
        {
            var job = _context.Jobs.FirstOrDefault(j => j.Id == id)
            ?? throw new Exception("Job doesn't exist.");
            _context.Jobs.Remove(job);
            _context.SaveChanges();
        }

        public Job GetJob(int Id)
        {
            var job = _context.Jobs.Where(j => j.Id == Id).FirstOrDefault();
            job.Documents = _context.Documents.Where(d => d.JobId == Id).ToList();
            return job;
        }

        public ICollection<Job> GetJobs(int UserId)
        {
            var User = _context.Users.Where(u => u.Id == UserId).FirstOrDefault();
            if(User == null)
            {
                return new List<Job>();
            }
            if (User.Role.ToString() == "Client")
            {
                var jobs = _context.Jobs.Where(j => j.Client_Id == UserId).OrderBy(j => j.Created_At).ToList();
                foreach (var job in jobs)
                {
                    job.Documents = _context.Documents.Where(d => d.JobId == job.Id).ToList();
                }
                return jobs;
            }
            if (User.Role.ToString() == "Engineer")
            {
                var jobs = _context.Jobs.Where(j => j.Engineer_Id == UserId).OrderBy(j => j.Created_At).ToList();
                foreach (var job in jobs)
                {
                    job.Documents = _context.Documents.Where(d => d.JobId == job.Id).ToList();
                }
                return jobs;
            }
            return _context.Jobs.OrderBy(j => j.Id).ToList();

        }

        public IEnumerable<Job> SearchJobs(string query)
        {
            //return _context.Jobs.Where(job => job.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            return _context.Jobs.Where(job => EF.Functions.Like(job.Name, $"%{query}%"));
        }

        public void UpdateJob(Job job)
        {
            var existingJob = _context.Jobs.FirstOrDefault(u => u.Id == job.Id)
            ?? throw new Exception("Job doesn't exist.");
            existingJob.Name = job.Name;
            existingJob.Description = job.Description;
            existingJob.Status = job.Status;
            existingJob.Deadline = job.Deadline;
            existingJob.Documents = job.Documents;
            _context.SaveChanges();
        }
    }
}
