using Microsoft.EntityFrameworkCore;
using InsuranceAPI.Models;

namespace InsuranceAPI.Data
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(
            DbContextOptions<InsuranceDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InsuranceProduct> InsuranceProducts { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //user to policy relationship
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.User)
                .WithMany(u => u.Policies)
                .HasForeignKey(p => p.UserId);
            //policy to product relationship
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.Product)
                .WithMany(p => p.Policies)
                .HasForeignKey(p => p.ProductId);
            //claim to policy relationship
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Policy)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PolicyId);
            // Ensure that the Email field in the User entity is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

           modelBuilder.Entity<Claim>()
                .ToTable(tb => tb.HasTrigger("trg_ClaimStatusAudit"));
        }
    }
}