using PFA_WebAPI.Models;

namespace PFA_WebAPI.Interfaces
{
    public interface IDocumentRepository
    {
        public Document GetDocument(int id);
        public void DeleteDocument(int id);
    }
}
