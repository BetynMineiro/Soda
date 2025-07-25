using Microsoft.EntityFrameworkCore;
using Soda.Domain.Entities;


namespace Soda.Postgres.EF.Adapter.Context;

public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
{
    public DbSet<Employer> Employers { get; set; }
    public DbSet<PhoneEmployer> PhoneEmployers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureEntityKeys(modelBuilder);
        ConfigureGuidDefaults(modelBuilder);
        ConfigureRelationships(modelBuilder);
    }

    private static void ConfigureEntityKeys(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employer>().HasKey(e => e.Id);
        modelBuilder.Entity<PhoneEmployer>().HasKey(e => e.Id);
    }

    private static void ConfigureGuidDefaults(ModelBuilder modelBuilder)
    {
        var entities = new[]
        {
            typeof(Employer)
        };

        foreach (var entity in entities)
        {
            modelBuilder.Entity(entity).Property("Id").HasDefaultValueSql("gen_random_uuid()");
        }
    }

    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhoneEmployer>(entity =>
        {
            entity
                .HasOne(p => p.Employer)
                .WithMany(e => e.PhoneNumbers)
                .HasForeignKey(p => p.EmployerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.Ignore(e => e.Age);
            entity.HasIndex(e => e.TaxDocument)
                .IsUnique()
                .HasDatabaseName("IX_Employer_TaxDocument_UQ");
            
            entity.Property(e => e.BirthDate)
                .HasColumnType("date");

            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}