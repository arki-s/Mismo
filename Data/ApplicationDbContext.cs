using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mismo.Models;

namespace Mismo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mismo.Models.OneOnOne> OneOnOne { get; set; } = default!;

        public DbSet<Mismo.Models.ApplicationUser>? Users { get; set; }
        public DbSet<Mismo.Models.Goal>? Goal { get; set; }
        public DbSet<Mismo.Models.Message>? Message { get; set; }
        public DbSet<Mismo.Models.Mood>? Mood { get; set; }
        public DbSet<Mismo.Models.Department>? Department { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}