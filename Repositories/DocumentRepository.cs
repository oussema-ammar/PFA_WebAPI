using Microsoft.EntityFrameworkCore;
using PFA_WebAPI.Data;
using PFA_WebAPI.Interfaces;
using PFA_WebAPI.Models;

namespace PFA_WebAPI.Repositories
{
    public class DocumentRepository: IDocumentRepository
    {
        private readonly DataContext _context;
        public DocumentRepository(DataContext context)
        {
            _context = context;
        }
        public bool Addjob(Job job)
        {
            _context.Jobs.Add(job);
            _context.SaveChanges();
            return true;
        }

        public void DeleteDocument(int id)
        {
            var doc = _context.Documents.FirstOrDefault(j => j.Id == id)
            ?? throw new Exception("Document doesn't exist.");
            _context.Documents.Remove(doc);
            _context.SaveChanges();
        }

        public Document GetDocument(int id)
        {
            return _context.Documents.Where(d => d.Id == id).FirstOrDefault()
            ?? throw new Exception("Document doesn't exist.");
        }
    }
}
