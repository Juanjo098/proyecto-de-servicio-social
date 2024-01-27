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

    public virtual DbSet<Alternativa> Alternativas { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<Carrera> Carreras { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<DocenteCargo> DocenteCargos { get; set; }

    public virtual DbSet<InfoPersonal> InfoPersonals { get; set; }

    public virtual DbSet<InformacionTitulacion> InformacionTitulacions { get; set; }

    public virtual DbSet<Opcione> Opciones { get; set; }

    public virtual DbSet<ProcesoTitulacion> ProcesoTitulacions { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alternativa>(entity =>
        {
            entity.HasKey(e => e.IdAlternativa).HasName("PRIMARY");

            entity.ToTable("alternativa");

            entity.Property(e => e.IdAlternativa).HasColumnName("id_alternativa");
            entity.Property(e => e.Alternativa1)
                .HasMaxLength(60)
                .HasColumnName("alternativa");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
        });

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
                .HasMaxLength(64)
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

            entity.HasIndex(e => e.IdJefeDpto, "id_jefe_dpto");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdDpto).HasColumnName("id_dpto");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdJefeDpto).HasColumnName("id_jefe_dpto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdJefeDptoNavigation).WithMany(p => p.Departamentos)
                .HasForeignKey(d => d.IdJefeDpto)
                .HasConstraintName("departamento_ibfk_1");
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
            entity.Property(e => e.Diminutivo)
                .HasMaxLength(16)
                .HasColumnName("diminutivo");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.IdDpto).HasColumnName("id_dpto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .HasColumnName("nombre");
            entity.Property(e => e.Titulo)
                .HasMaxLength(128)
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

        modelBuilder.Entity<InformacionTitulacion>(entity =>
        {
            entity.HasKey(e => e.IdInfoTitulacion).HasName("PRIMARY");

            entity.ToTable("informacion_titulacion");

            entity.HasIndex(e => e.NoControl, "no_control").IsUnique();

            entity.Property(e => e.IdInfoTitulacion).HasColumnName("id_info_titulacion");
            entity.Property(e => e.Alternativa)
                .HasMaxLength(60)
                .HasColumnName("alternativa");
            entity.Property(e => e.FechaAarp)
                .HasColumnType("datetime")
                .HasColumnName("fecha_aarp");
            entity.Property(e => e.FechaArp)
                .HasColumnType("datetime")
                .HasColumnName("fecha_arp");
            entity.Property(e => e.FechaCni)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cni");
            entity.Property(e => e.FechaSt)
                .HasColumnType("datetime")
                .HasColumnName("fecha_st");
            entity.Property(e => e.FechaVecimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vecimiento");
            entity.Property(e => e.NoControl)
                .HasMaxLength(10)
                .HasColumnName("no_control");
            entity.Property(e => e.Presidente).HasColumnName("presidente");
            entity.Property(e => e.Producto)
                .HasMaxLength(60)
                .HasColumnName("producto");
            entity.Property(e => e.Proyecto)
                .HasMaxLength(256)
                .HasColumnName("proyecto");
            entity.Property(e => e.Secretario).HasColumnName("secretario");
            entity.Property(e => e.Suplente).HasColumnName("suplente");
            entity.Property(e => e.Vocal).HasColumnName("vocal");

            entity.HasOne(d => d.NoControlNavigation).WithOne(p => p.InformacionTitulacion)
                .HasForeignKey<InformacionTitulacion>(d => d.NoControl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("informacion_titulacion_ibfk_1");
        });

        modelBuilder.Entity<Opcione>(entity =>
        {
            entity.HasKey(e => e.IdOpcion).HasName("PRIMARY");

            entity.ToTable("opciones");

            entity.Property(e => e.IdOpcion).HasColumnName("id_opcion");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.Opcion)
                .HasMaxLength(60)
                .HasColumnName("opcion");
        });

        modelBuilder.Entity<ProcesoTitulacion>(entity =>
        {
            entity.HasKey(e => e.IdProceso).HasName("PRIMARY");

            entity.ToTable("proceso_titulacion");

            entity.HasIndex(e => e.NoControl, "no_control").IsUnique();

            entity.Property(e => e.IdProceso).HasColumnName("id_proceso");
            entity.Property(e => e.Asnc).HasColumnName("asnc");
            entity.Property(e => e.Caii).HasColumnName("caii");
            entity.Property(e => e.Cb).HasColumnName("cb");
            entity.Property(e => e.Cl).HasColumnName("cl");
            entity.Property(e => e.Cni).HasColumnName("cni");
            entity.Property(e => e.Curp).HasColumnName("curp");
            entity.Property(e => e.Lp).HasColumnName("lp");
            entity.Property(e => e.NoControl)
                .HasMaxLength(10)
                .HasColumnName("no_control");
            entity.Property(e => e.Oi).HasColumnName("oi");
            entity.Property(e => e.Paso1).HasColumnName("paso1");
            entity.Property(e => e.Pro).HasColumnName("pro");
            entity.Property(e => e.Rfc).HasColumnName("rfc");
            entity.Property(e => e.Rp).HasColumnName("rp");
            entity.Property(e => e.Rps).HasColumnName("rps");
            entity.Property(e => e.Scni).HasColumnName("scni");
            entity.Property(e => e.Sl).HasColumnName("sl");
            entity.Property(e => e.St).HasColumnName("st");

            entity.HasOne(d => d.NoControlNavigation).WithOne(p => p.ProcesoTitulacion)
                .HasForeignKey<ProcesoTitulacion>(d => d.NoControl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proceso_titulacion_ibfk_1");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PRIMARY");

            entity.ToTable("producto");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Hab)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("hab");
            entity.Property(e => e.Producto1)
                .HasMaxLength(60)
                .HasColumnName("producto");
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
