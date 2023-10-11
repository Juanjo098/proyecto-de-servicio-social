using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Titulacion.Models;

public partial class TitulacionContext : DbContext
{
    public TitulacionContext()
    {
    }

    public TitulacionContext(DbContextOptions<TitulacionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<Carrera> Carreras { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<DocenteCargo> DocenteCargos { get; set; }

    public virtual DbSet<InfoPersonal> InfoPersonals { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargo).HasName("PRIMARY");

            entity.ToTable("cargo");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Carrera>(entity =>
        {
            entity.HasKey(e => e.IdCarrera).HasName("PRIMARY");

            entity.ToTable("carrera");

            entity.HasIndex(e => e.IdCarrera, "id_carrera").IsUnique();

            entity.HasIndex(e => e.IdDpto, "id_dpto");

            entity.Property(e => e.IdCarrera).HasColumnName("id_carrera");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdDpto).HasColumnName("id_dpto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(32)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDptoNavigation).WithMany(p => p.Carreras)
                .HasForeignKey(d => d.IdDpto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carrera_ibfk_1");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDpto).HasName("PRIMARY");

            entity.ToTable("departamento");

            entity.HasIndex(e => e.IdDpto, "id_dpto").IsUnique();

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdDpto).HasColumnName("id_dpto");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasKey(e => e.IdDocente).HasName("PRIMARY");

            entity.ToTable("docente");

            entity.HasIndex(e => e.Cedula, "cedula").IsUnique();

            entity.HasIndex(e => e.IdDpto, "id_dpto");

            entity.Property(e => e.IdDocente).HasColumnName("id_docente");
            entity.Property(e => e.Cedula)
                .HasMaxLength(16)
                .HasColumnName("cedula");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdDpto).HasColumnName("id_dpto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .HasColumnName("nombre");
            entity.Property(e => e.Titulo)
                .HasMaxLength(64)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdDptoNavigation).WithMany(p => p.Docentes)
                .HasForeignKey(d => d.IdDpto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("docente_ibfk_1");
        });

        modelBuilder.Entity<DocenteCargo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("docente_cargo");

            entity.HasIndex(e => e.IdCargo, "id_cargo");

            entity.HasIndex(e => e.IdDocente, "id_docente");

            entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
            entity.Property(e => e.IdDocente).HasColumnName("id_docente");

            entity.HasOne(d => d.IdCargoNavigation).WithMany()
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("docente_cargo_ibfk_2");

            entity.HasOne(d => d.IdDocenteNavigation).WithMany()
                .HasForeignKey(d => d.IdDocente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("docente_cargo_ibfk_1");
        });

        modelBuilder.Entity<InfoPersonal>(entity =>
        {
            entity.HasKey(e => e.NoControl).HasName("PRIMARY");

            entity.ToTable("info_personal");

            entity.HasIndex(e => e.IdCarrera, "id_carrera");

            entity.HasIndex(e => e.IdUsuario, "id_usuario").IsUnique();

            entity.HasIndex(e => e.NoControl, "no_control").IsUnique();

            entity.Property(e => e.NoControl)
                .HasMaxLength(10)
                .HasColumnName("no_control");
            entity.Property(e => e.ApMaterno)
                .HasMaxLength(32)
                .HasColumnName("ap_materno");
            entity.Property(e => e.ApPaterno)
                .HasMaxLength(32)
                .HasColumnName("ap_paterno");
            entity.Property(e => e.Direccion)
                .HasMaxLength(128)
                .HasColumnName("direccion");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdCarrera).HasColumnName("id_carrera");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(64)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(16)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdCarreraNavigation).WithMany(p => p.InfoPersonals)
                .HasForeignKey(d => d.IdCarrera)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("info_personal_ibfk_2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.InfoPersonal)
                .HasForeignKey<InfoPersonal>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("info_personal_ibfk_1");
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdTipoUsuario).HasName("PRIMARY");

            entity.ToTable("tipo_usuario");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdTipoUsuario).HasColumnName("id_tipo_usuario");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.Nombre)
                .HasMaxLength(16)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "correo").IsUnique();

            entity.HasIndex(e => e.IdTipoUsuario, "id_tipo_usuario");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(64)
                .HasColumnName("correo");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdTipoUsuario)
                .HasDefaultValueSql("'3'")
                .HasColumnName("id_tipo_usuario");
            entity.Property(e => e.MensajesHab)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("mensajes_hab");
            entity.Property(e => e.Nombre)
                .HasMaxLength(64)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdTipoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
