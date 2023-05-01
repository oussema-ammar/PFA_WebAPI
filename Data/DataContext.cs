using PFA_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PFA_WebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Job> Jobs { get; set; }
    }
}