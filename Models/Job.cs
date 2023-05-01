using PFA_WebAPI.Validators;

namespace PFA_WebAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime Created_At { get; set; }= DateTime.Now;
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7);
        [UserIsClient]
        public int Client_Id { get; set; }
        [UserIsEngineer]
        public int Engineer_Id { get; set; }
        public ICollection<Document> Documents { get; set; }= new List<Document>();

    }
}
