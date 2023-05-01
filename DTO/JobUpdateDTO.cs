namespace PFA_WebAPI.DTO
{
    public class JobUpdateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
    }
}
