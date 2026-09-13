using Microsoft.EntityFrameworkCore;

namespace Proyecto2_1_1548_0877.Models
{
    public class PetsContext : DbContext
    {
        public PetsContext(DbContextOptions<PetsContext> options)
             : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employes> Employes { get; set; }
        public DbSet<Pets> Pets { get; set; }
        public DbSet<Procedures> Procedures { get; set; }
        public DbSet<Reports> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //virtual method of the base DbContext class
        {
            base.OnModelCreating(modelBuilder);

            // Customer config
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer"); //table
                entity.HasKey(e => e.IdCustomer); //Primary key
                entity.Property(e => e.IdCustomer).ValueGeneratedNever(); //do not generate an automatic value
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.State).IsRequired().HasMaxLength(50);
                entity.Property(e => e.City).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ContactPreference)
                    .HasMaxLength(20)
                    .HasConversion<string>(); //enum to string
            });

            // Employee config
            modelBuilder.Entity<Employes>(entity =>
            {
                entity.ToTable("Employes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(20);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.SalaryPerDay).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.TypeEmploye)
                    .HasMaxLength(20)
                    .HasConversion<string>();
            });

            // Pet config
            modelBuilder.Entity<Pets>(entity =>
            {
                entity.ToTable("Pets");
                entity.HasKey(e => e.PetId);
                entity.Property(e => e.PetId).ValueGeneratedOnAdd();
                entity.Property(e => e.PetName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AnimalSpecies)
                    .HasMaxLength(20)
                    .HasConversion<string>();
                entity.Property(e => e.Breed).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PhoneOwner).HasMaxLength(15);
                entity.Property(e => e.EmailOwner).HasMaxLength(100);


            });

            // Procedure config
            modelBuilder.Entity<Procedures>(entity =>
            {
                entity.ToTable("Procedures");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ProcedureType)
                    .HasMaxLength(50)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.BasePrice)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Ignore(e => e.VAT);
                entity.Ignore(e => e.TotalPrice);

                // Relation with Customer
                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(e => e.IdCustomer) //foreign key
                    .OnDelete(DeleteBehavior.Restrict);

                // RELATION WITH PET
                entity.HasOne(p => p.Pets)
                    .WithMany()
                    .HasForeignKey(p => p.PetId) //foreign key
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });


            modelBuilder.Entity<Reports>(entity =>
            {
                entity.ToTable("Reports");

                entity.HasKey(e => e.IdReport);
                entity.Property(e => e.IdReport).ValueGeneratedOnAdd();

                entity.Property(e => e.ProcedureType)
                      .HasMaxLength(100)
                      .HasConversion<string>();

                entity.Property(e => e.ProcedureStatus)
                      .HasMaxLength(50)
                      .HasConversion<string>();

                entity.Property(e => e.BasePrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.VAT).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");

                // Relations
                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.IdCustomer)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Pets)
                      .WithMany()
                      .HasForeignKey(r => r.PetId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
