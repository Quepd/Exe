using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Wpf.Models;

public partial class SysManageContext : DbContext
{
    public SysManageContext()
    {
    }

    public SysManageContext(DbContextOptions<SysManageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
											.AddJsonFile("jsconfig1.json", optional: true, reloadOnChange: true);
		IConfiguration configuration = builder.Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC07C980B29A");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Name, "UQ__Account__737584F686EEF139").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07F5642ED5");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__737584F698A98B56").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Item__3214EC0756AE7319");

            entity.ToTable("Item");

            entity.HasIndex(e => e.Name, "UQ__Item__737584F650440482").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Item__Category__412EB0B6");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07C13ECD6C");

            entity.ToTable("Order");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderItem");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__OrderItem__ItemI__4222D4EF");

            //entity.HasOne(d => d.Order).WithMany()
            //    .HasForeignKey(d => d.OrderId)
            //    .HasConstraintName("FK__OrderItem__Order__4316F928");
            entity.HasOne(d => d.Item).WithMany()
    .HasForeignKey(d => d.ItemId)
    .OnDelete(DeleteBehavior.Cascade) // Nếu muốn quy định cách xóa
    .HasConstraintName("FK_OrderItem_Item");

        });
		modelBuilder.Entity<OrderItem>()
	   .HasKey(oi => new { oi.OrderId, oi.ItemId });

		OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
