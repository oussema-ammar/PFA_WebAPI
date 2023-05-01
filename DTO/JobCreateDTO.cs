using PFA_WebAPI.Validators;

namespace PFA_WebAPI.DTO
{
    public class JobCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7);
        [UserIsEngineer]
        public int Engineer_Id { get; set; }
    }
}
