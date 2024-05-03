using System;
using System.Collections.Generic;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Demo_Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo_Test.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Student> Students { get; set; }  

    public virtual DbSet<StudentClass> StudentClasses { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("User ID = postgres;Password=shlok123;Server=localhost;Port=5432;Database=Demo-Test;Integrated Security=true;Pooling=true;");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            // Check if the environment is development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                // Use development Azure Key Vault URL
                string developmentKeyVaultUri = config["AzureKeyVault:DevelopmentVaultUri"];
                if (!string.IsNullOrEmpty(developmentKeyVaultUri))
                {
                    var client = new SecretClient(new Uri(developmentKeyVaultUri), new DefaultAzureCredential());

                    // Retrieve the secret from Azure Key Vault and override the connection string
                    KeyVaultSecret secret = client.GetSecret("DBConnectionString-DemoTest");

                    // Use the secret value as the connection string
                    connectionString = secret.Value;
                }
            }
            else
            {
                // Use production Azure Key Vault URL
                string productionKeyVaultUri = config["AzureKeyVault:ProductionVaultUri"];
                if (!string.IsNullOrEmpty(productionKeyVaultUri))
                {
                    var client = new SecretClient(new Uri(productionKeyVaultUri), new DefaultAzureCredential());

                    // Retrieve the secret from Azure Key Vault and override the connection string
                    KeyVaultSecret secret = client.GetSecret("DBConnectionString-DemoTest");

                    // Use the secret value as the connection string
                    connectionString = secret.Value;
                }
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Rollno).HasName("students_pkey");

            entity.HasOne(d => d.Class).WithMany(p => p.Students).HasConstraintName("students_classid_fkey");
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_class_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
