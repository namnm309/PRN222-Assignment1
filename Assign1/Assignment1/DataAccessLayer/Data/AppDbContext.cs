using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Database migration sẽ được gọi ở Program.cs để tránh lỗi lúc design-time
        }
        // Constructor mặc định cho testing/migration
        public AppDbContext()
        {
        }
        // Khai báo các DbSet Entity - mỗi DbSet đại diện cho một bảng trong cơ sở dữ liệu 
        public DbSet<Users> Users { get; set; }
        
        public DbSet<Product> Product { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<Customer> Customer { get; set; }
        
        public DbSet<Dealer> Dealer { get; set; }
        
        public DbSet<Category> Categorie { get; set; }
        
        public DbSet<Order> Order { get; set; }
        
        public DbSet<Feedback> Feedback { get; set; }
        
        public DbSet<Promotion> Promotion { get; set; }
        
        public DbSet<TestDrive> TestDrive { get; set; }

        // Cấu hình chi tiết Entity - sử dụng khi cần cấu hình phức tạp ngoài Data Annotations 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ Product - Brand (Many-to-One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Customer (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Dealer (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Dealer)
                .WithMany()
                .HasForeignKey(o => o.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Product (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Feedback - Customer (Many-to-One)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany()
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Feedback - Product (Many-to-One)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Customer (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Product (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Dealer (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Dealer)
                .WithMany()
                .HasForeignKey(t => t.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình các thuộc tính bắt buộc
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.ModelName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.color).HasMaxLength(50);
                entity.Property(e => e.varian).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.discount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.status).HasMaxLength(20);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(1000);
               
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.description).HasMaxLength(1000);
            });

            modelBuilder.Entity<TestDrive>(entity =>
            {
                entity.Property(e => e.Status).HasMaxLength(20);
            });
        }
        }
}
