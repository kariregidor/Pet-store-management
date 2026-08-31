using Microsoft.EntityFrameworkCore;

namespace Proyecto1_1_1548_0877.Models
{
    public class PetsContext : DbContext
    {
        public PetsContext(DbContextOptions<PetsContext> options)
             : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<Mascotas> Mascotas { get; set; }
        public DbSet<Procedimientos> Procedimientos { get; set; }
        public DbSet<Reportes> Reportes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //método virtual de la clase base DbContext
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes"); //tabla
                entity.HasKey(e => e.IdCliente); //Llave primaria
                entity.Property(e => e.IdCliente).ValueGeneratedNever(); //no generer valor automatico
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Provincia).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Canton).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Distrito).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DireccionExacta).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PreferenciaContacto)
                    .HasMaxLength(20)
                    .HasConversion<string>(); //enum a string
            });

            // Configuración de Empleado
            modelBuilder.Entity<Empleados>(entity =>
            {
                entity.ToTable("Empleados");
                entity.HasKey(e => e.Cedula);
                entity.Property(e => e.Cedula).HasMaxLength(20);
                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.FechaIngreso).IsRequired();
                entity.Property(e => e.SalarioDia).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.TipoEmpleado)
                    .HasMaxLength(20)
                    .HasConversion<string>();
            });

            // Configuración de Mascota
            modelBuilder.Entity<Mascotas>(entity =>
            {
                entity.ToTable("Mascotas");
                entity.HasKey(e => e.MascotaId);
                entity.Property(e => e.MascotaId).ValueGeneratedOnAdd();
                entity.Property(e => e.NombreMascota).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Especie)
                    .HasMaxLength(20)
                    .HasConversion<string>();
                entity.Property(e => e.Raza).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TelefonoDueno).HasMaxLength(15);
                entity.Property(e => e.EmailDueno).HasMaxLength(100);


            });

            // Configuración de Procedimiento
            modelBuilder.Entity<Procedimientos>(entity =>
            {
                entity.ToTable("Procedimientos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.TipoProcedimiento)
                    .HasMaxLength(50)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.PrecioBase)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Ignore(e => e.IVA);
                entity.Ignore(e => e.PrecioTotal);

                // Relación con Cliente
                entity.HasOne<Cliente>()
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente) //llave foranea
                    .OnDelete(DeleteBehavior.Restrict);

                // RELACIÓN CON MASCOTA
                entity.HasOne(p => p.Mascota)
                    .WithMany()
                    .HasForeignKey(p => p.MascotaId) //llave fornaea
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });


            modelBuilder.Entity<Reportes>(entity =>
            {
                entity.ToTable("Reportes");

                entity.HasKey(e => e.IdReporte);
                entity.Property(e => e.IdReporte).ValueGeneratedOnAdd();

                entity.Property(e => e.TipoProcedimiento)
                      .HasMaxLength(100)
                      .HasConversion<string>();

                entity.Property(e => e.EstadoProcedimiento)
                      .HasMaxLength(50)
                      .HasConversion<string>();

                entity.Property(e => e.PrecioBase).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IVA).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PrecioTotal).HasColumnType("decimal(10,2)");

                // Relaciones
                entity.HasOne(r => r.Cliente)
                      .WithMany()
                      .HasForeignKey(r => r.IdCliente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Mascota)
                      .WithMany()
                      .HasForeignKey(r => r.MascotaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
