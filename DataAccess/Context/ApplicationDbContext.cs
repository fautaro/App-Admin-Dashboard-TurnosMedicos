﻿using DataAccess.Models;
using DataAccess.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {   
        // Define propiedades DbSet para cada entidad que deseas mapear a tablas en la base de datos
        public DbSet<Profesion> Profesion { get; set; }
        public DbSet<Profesional> Profesional { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<AgendaBloqueada> AgendaBloqueada { get; set; }
        public DbSet<Horario> Horario { get; set; }
        public DbSet<UsuarioProfesional> UsuarioProfesional { get; set; }
        public DbSet<Notificacion> Notificacion { get; set; }
        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<Evento> Evento { get; set; }

        //ViewModels
        public DbSet<AdminDashboardResult> AdminDashboardResults { get; set; }




        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Opcional: Si deseas personalizar cómo se mapean las entidades a las tablas en la base de datos, puedes configurarlas aquí
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.ToTable("Turno"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Turno_Id); // Clave primaria
                entity.Property(e => e.Turno_Id).HasColumnName("Turno_Id").IsRequired(); // Propiedad para el campo Turno_Id
                entity.Property(e => e.FechaHora).HasColumnName("FechaHora").IsRequired(); // Propiedad para el campo FechaHora
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Apellido).HasColumnName("Apellido").HasMaxLength(50); // Propiedad para el campo Apellido con límite de 50 caracteres
                entity.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(50); // Propiedad para el campo Telefono con límite de 50 caracteres
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(50); // Propiedad para el campo Email con límite de 50 caracteres
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired(); // Propiedad para el campo Activo
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.Link).HasColumnName("Link").HasMaxLength(50); // Propiedad para el campo Link con límite de 50 caracteres
                entity.Property(e => e.Token).HasColumnName("Token").HasMaxLength(100); // Propiedad para el campo Link con límite de 50 caracteres

            });


            modelBuilder.Entity<Profesion>(entity =>
            {
                entity.ToTable("Profesion"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Profesion_Id); // Clave primaria
                entity.Property(e => e.Profesion_Id).HasColumnName("Profesion_Id").ValueGeneratedOnAdd(); // Propiedad para el campo Profesion_Id con autoincremento
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Rubro).HasColumnName("Rubro").HasMaxLength(50); // Propiedad para el campo Rubro con límite de 50 caracteres
            });

            // Configuración de la entidad Profesional
            modelBuilder.Entity<Profesional>(entity =>
            {
                entity.ToTable("Profesional"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Profesional_Id); // Clave primaria
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Apellido).HasColumnName("Apellido").HasMaxLength(50); // Propiedad para el campo Apellido con límite de 50 caracteres
                entity.Property(e => e.Alias).HasColumnName("Alias").HasMaxLength(15); // Propiedad para el campo Alias con límite de 15 caracteres
                entity.Property(e => e.Profesion_Id).HasColumnName("Profesion_Id").IsRequired(); // Propiedad para el campo Profesion_Id
                entity.Property(e => e.Usuario_Id).HasColumnName("Usuario_Id").IsRequired(); // Propiedad para el campo Usuario_Id
                entity.Property(e => e.Activo).HasColumnName("Activo"); // Propiedad para el campo Activo
                entity.Property(e => e.Titulo).HasColumnName("Titulo").HasMaxLength(100); // Propiedad para el campo Titulo con límite de 100 caracteres
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(500); // Propiedad para el campo Descripcion con límite de 500 caracteres
                entity.Property(e => e.Imagen).HasColumnName("Imagen"); // Propiedad para el campo Imagen (varbinary)
                entity.Property(e => e.Intervalo).HasColumnName("Intervalo"); // Propiedad para el campo Intervalo


            });


            modelBuilder.Entity<AgendaBloqueada>(entity =>
            {
                entity.ToTable("AgendaBloqueada"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.AgendaBloqueada_Id); // Clave primaria
                entity.Property(e => e.AgendaBloqueada_Id).HasColumnName("AgendaBloqueada_Id").ValueGeneratedOnAdd(); // Propiedad para el campo AgendaBloqueada_Id con autoincremento
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.FechaDesde).HasColumnName("FechaDesde").IsRequired(); // Propiedad para el campo FechaDesde
                entity.Property(e => e.FechaHasta).HasColumnName("FechaHasta").IsRequired(); // Propiedad para el campo FechaHasta
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired(); // Propiedad para el campo Activo
            });

            modelBuilder.Entity<Horario>(entity =>
            {
                entity.ToTable("Horario"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Horario_Id); // Clave primaria
                entity.Property(e => e.Horario_Id).HasColumnName("Horario_Id"); // Mapeo de propiedad
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id"); // Mapeo de propiedad
                entity.Property(e => e.Hora).HasColumnName("Hora");
            });

            modelBuilder.Entity<UsuarioProfesional>(entity =>
            {
                entity.ToTable("UsuarioProfesional"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.UsuarioProfesional_Id); // Clave primaria
                entity.Property(e => e.User_Id).HasColumnName("User_Id"); // Mapeo de propiedad
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id"); // Mapeo de propiedad
            });

            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("Notificacion");
                entity.HasKey(e => e.Notificacion_Id);
                entity.Property(e => e.Notificacion_Id).HasColumnName("Notificacion_Id").IsRequired();
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired();
                entity.Property(e => e.Titulo).HasColumnName("Titulo").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(500);
                entity.Property(e => e.Leido).HasColumnName("Leido").IsRequired();
                entity.Property(e => e.FechaHoraEvento).HasColumnName("FechaHoraEvento");
                entity.Property(e => e.Eliminado).HasColumnName("Eliminado");

            });
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.ToTable("Administrador"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Administrador_Id); // Clave primaria
                entity.Property(e => e.Administrador_Id).HasColumnName("Administrador_Id"); // Mapeo de propiedad
                entity.Property(e => e.Admin_Nombre).HasColumnName("Admin_Nombre"); // Mapeo de propiedad
                entity.Property(e => e.Admin_Email).HasColumnName("Admin_Email"); // Mapeo de propiedad
            });

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("Evento"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Evento_Id); // Clave primaria
                entity.Property(e => e.Evento_Id).HasColumnName("Evento_Id"); // Mapeo de propiedad
                entity.Property(e => e.Usuario_Id).HasColumnName("Usuario_Id"); // Mapeo de propiedad
                entity.Property(e => e.Detalle).HasColumnName("Detalle"); // Mapeo de propiedad
                entity.Property(e => e.Entidad).HasColumnName("Entidad"); // Mapeo de propiedad
                entity.Property(e => e.FechaHora).HasColumnName("FechaHora"); // Mapeo de propiedad
            });


        }
    }
}
