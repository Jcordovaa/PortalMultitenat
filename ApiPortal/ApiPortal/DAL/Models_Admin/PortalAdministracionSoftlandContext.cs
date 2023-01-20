using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class PortalAdministracionSoftlandContext : DbContext
    {
        public PortalAdministracionSoftlandContext()
        {
        }

        public PortalAdministracionSoftlandContext(DbContextOptions<PortalAdministracionSoftlandContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AreaComercial> AreaComercials { get; set; } = null!;
        public virtual DbSet<CsvEmpresasSii> CsvEmpresasSiis { get; set; } = null!;
        public virtual DbSet<EmpresaEstado> EmpresaEstados { get; set; } = null!;
        public virtual DbSet<EmpresasPortal> EmpresasPortals { get; set; } = null!;
        public virtual DbSet<Implementador> Implementadors { get; set; } = null!;
        public virtual DbSet<LineaProducto> LineaProductos { get; set; } = null!;
        public virtual DbSet<Plane> Planes { get; set; } = null!;
        public virtual DbSet<RolesPortal> RolesPortals { get; set; } = null!;
        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<UsuariosPortal> UsuariosPortals { get; set; } = null!;
        public virtual DbSet<UsuariosPortalToken> UsuariosPortalTokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=innova.zapto.org,1435;Database=PortalAdministracionSoftland;User Id=sa;Password=204709cejA;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AreaComercial>(entity =>
            {
                entity.HasKey(e => e.IdArea);

                entity.ToTable("AreaComercial");

                entity.Property(e => e.IdArea).ValueGeneratedNever();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CsvEmpresasSii>(entity =>
            {
                entity.HasKey(e => e.Rut);

                entity.ToTable("CSV_EmpresasSii");

                entity.Property(e => e.Rut)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCarga).HasColumnType("date");

                entity.Property(e => e.FechaResolucion).HasColumnType("date");

                entity.Property(e => e.Mail).IsUnicode(false);

                entity.Property(e => e.RazonSocial).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);
            });

            modelBuilder.Entity<EmpresaEstado>(entity =>
            {
                entity.HasKey(e => e.IdEstado);

                entity.ToTable("Empresa_Estado");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmpresasPortal>(entity =>
            {
                entity.HasKey(e => e.IdEmpresa);

                entity.ToTable("EmpresasPortal");

                entity.Property(e => e.FechaInicioContrato).HasColumnType("date");

                entity.Property(e => e.FechaInicioImplementacion).HasColumnType("date");

                entity.Property(e => e.FechaTerminoContrato).HasColumnType("date");

                entity.Property(e => e.FechaTerminoImplementacion).HasColumnType("date");

                entity.Property(e => e.OtImplementacion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OT_Implementacion");

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAreaComercialNavigation)
                    .WithMany(p => p.EmpresasPortals)
                    .HasForeignKey(d => d.IdAreaComercial)
                    .HasConstraintName("FK_EmpresasPortal_AreaComercial");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.EmpresasPortals)
                    .HasForeignKey(d => d.IdEstado)
                    .HasConstraintName("FK_EmpresasPortal_Empresa_Estado");

                entity.HasOne(d => d.IdImplementadorNavigation)
                    .WithMany(p => p.EmpresasPortals)
                    .HasForeignKey(d => d.IdImplementador)
                    .HasConstraintName("FK_EmpresasPortal_Implementador");

                entity.HasOne(d => d.IdLineaProductoNavigation)
                    .WithMany(p => p.EmpresasPortals)
                    .HasForeignKey(d => d.IdLineaProducto)
                    .HasConstraintName("FK_EmpresasPortal_LineaProducto");

                entity.HasOne(d => d.IdPlanNavigation)
                    .WithMany(p => p.EmpresasPortals)
                    .HasForeignKey(d => d.IdPlan)
                    .HasConstraintName("FK_EmpresasPortal_Planes");
            });

            modelBuilder.Entity<Implementador>(entity =>
            {
                entity.HasKey(e => e.IdImplementador);

                entity.ToTable("Implementador");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LineaProducto>(entity =>
            {
                entity.HasKey(e => e.IdLinea);

                entity.ToTable("LineaProducto");

                entity.Property(e => e.IdLinea).ValueGeneratedNever();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Plane>(entity =>
            {
                entity.HasKey(e => e.IdPlan);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolesPortal>(entity =>
            {
                entity.HasKey(e => e.IdRol);

                entity.ToTable("RolesPortal");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.IdTenant);

                entity.ToTable("Tenant");

                entity.Property(e => e.ConnectionString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Dominio)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Identifier)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Tenants)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("FK_Tenant_EmpresasPortal");
            });

            modelBuilder.Entity<UsuariosPortal>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.ToTable("UsuariosPortal");

                entity.Property(e => e.Apellido)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Avatar)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.ClaveHash)
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.ClaveSalt)
                    .HasMaxLength(128)
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdImplementadorNavigation)
                    .WithMany(p => p.UsuariosPortals)
                    .HasForeignKey(d => d.IdImplementador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UsuariosPortal_Implementador");

                entity.HasOne(d => d.IdRolNavigation)
                    .WithMany(p => p.UsuariosPortals)
                    .HasForeignKey(d => d.IdRol)
                    .HasConstraintName("FK_UsuariosPortal_RolesPortal");
            });

            modelBuilder.Entity<UsuariosPortalToken>(entity =>
            {
                entity.HasKey(e => e.IdToken);

                entity.ToTable("UsuariosPortalToken");

                entity.Property(e => e.Token)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.TokenCreated).HasColumnType("datetime");

                entity.Property(e => e.TokenExpires).HasColumnType("datetime");

                entity.Property(e => e.TokenRefresh)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.TokenRefreshCreated).HasColumnType("datetime");

                entity.Property(e => e.TokenRefreshExpires).HasColumnType("datetime");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuariosPortalTokens)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK_UsuariosPortalToken_UsuariosPortal");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
