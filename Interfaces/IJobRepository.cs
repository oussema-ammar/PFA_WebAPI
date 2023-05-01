using PFA_WebAPI.Models;

namespace PFA_WebAPI.Interfaces
{
    public interface IJobRepository
    {
        ICollection<Job> GetJobs(int UserId);
        public Job GetJob(int Id);
        public bool Addjob(Job job);
        public void UpdateJob(Job job);
        public void DeleteJob(int id);
        public IEnumerable<Job> SearchJobs(string query);
    }
}
