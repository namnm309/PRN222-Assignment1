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

        // Cấu hình chi tiết Entity - sử dụng khi cần cấu hình phức tạp ngoài Data Annotations 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình chi tiết cho Entity Users - override các thuộc tính đã có trong Data Annotations
            modelBuilder.Entity<Users>(entity =>
            {
               
            });
        }
        }
}
