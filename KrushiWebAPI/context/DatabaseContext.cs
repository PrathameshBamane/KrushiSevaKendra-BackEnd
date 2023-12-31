﻿using System;
using System.Collections.Generic;
using KrushiWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KrushiWebAPI.context;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<Recommendation> Recommendations { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-U7PEKMP1\\SQLEXPRESS;Initial Catalog=KrushiDB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Farmer>(entity =>
        {
            entity.HasOne(d => d.Town).WithMany(p => p.Farmers).HasConstraintName("FK_farmers_towns");
        });

        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.HasOne(d => d.Crop).WithMany(p => p.Recommendations).HasConstraintName("FK_recommendations_crops");

            entity.HasOne(d => d.Farmer).WithMany(p => p.Recommendations).HasConstraintName("FK_recommendations_farmers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
