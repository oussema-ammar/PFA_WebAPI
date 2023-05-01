namespace PFA_WebAPI.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Uploaded_At { get; set; }= DateTime.Now;
        public int JobId { get; set; }
    }
}
