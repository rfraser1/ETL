using System;
using System.Collections.Generic;
using ETL.DBModels;
using Microsoft.EntityFrameworkCore;

namespace ETL.Data;

public partial class RainfallContext : DbContext
{
    public RainfallContext()
    {
    }

    public RainfallContext(DbContextOptions<RainfallContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnnualSummary> AnnualSummaries { get; set; }

    public virtual DbSet<Rainfall> Rainfalls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AnnualSummary");

            entity.Property(e => e.session)
                .HasMaxLength(256)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rainfall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rainfall__3214EC07BC9C491E");

            entity.ToTable("Rainfall");

            entity.Property(e => e.date).HasColumnType("date");
            entity.Property(e => e.session)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.value).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.xref).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.yref).HasColumnType("decimal(18, 3)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
