using Microsoft.EntityFrameworkCore;
using RapidCMSTests.Entities;

namespace RapidCMSTests.EFCore
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<PersonCountry> PersonCountry { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonCountry>().HasKey(x => new { x.CountryId, x.PersonId });

            modelBuilder.Entity<PersonCountry>()
                .HasOne(x => x.Person)
                .WithMany(x => x.Countries)
                .HasForeignKey(x => x.PersonId);

            modelBuilder.Entity<PersonCountry>()
                .HasOne(x => x.Country)
                .WithMany(x => x.People)
                .HasForeignKey(x => x.CountryId);

            modelBuilder.Entity<Person>()
                .HasOne(x => x.Parent)
                .WithMany()
                .IsRequired(false);
        }
    }
}
