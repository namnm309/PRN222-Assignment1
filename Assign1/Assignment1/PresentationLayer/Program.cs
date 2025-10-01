using DataAccessLayer.Data;
using DataAccessLayer.Repository;
using BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using System;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Cấu hình DbContext (đăng ký DI trước khi build app)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository và Services cho Authen
            builder.Services.AddScoped<IAuthen, Authen>();
            builder.Services.AddScoped<IAuthenService, AuthenService>();

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Seed dữ liệu mẫu khi khởi động (chỉ cho môi trường Development)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<AppDbContext>();
                    // Áp dụng migration (nếu có)
                    db.Database.Migrate();

                    // Seed Brand/Product cơ bản nếu trống
                    if (!db.Brand.Any())
                    {
                        var vinfast = new DataAccessLayer.Entities.Brand
                        {
                            Id = Guid.NewGuid(),
                            Name = "VinFast",
                            Country = "Việt Nam",
                            Description = "Thương hiệu xe điện Việt Nam",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        var tesla = new DataAccessLayer.Entities.Brand
                        {
                            Id = Guid.NewGuid(),
                            Name = "Tesla",
                            Country = "USA",
                            Description = "Electric Vehicle Pioneer",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Brand.AddRange(vinfast, tesla);
                        db.SaveChanges();

                        var products = new List<DataAccessLayer.Entities.Product>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Sku = "VF-e34",
                                Name = "VinFast VF e34",
                                Description = "SUV cỡ C, nhiều công nghệ hỗ trợ lái.",
                                Price = 690_000_000m,
                                StockQuantity = 10,
                                IsActive = true,
                                BrandId = vinfast.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            },
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Sku = "VF-8",
                                Name = "VinFast VF 8",
                                Description = "SUV điện 2 hàng ghế, tầm hoạt động tốt.",
                                Price = 1_090_000_000m,
                                StockQuantity = 5,
                                IsActive = true,
                                BrandId = vinfast.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            },
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Sku = "TSL-3",
                                Name = "Tesla Model 3",
                                Description = "Sedan điện hiệu năng/giá tốt.",
                                Price = 1_300_000_000m,
                                StockQuantity = 7,
                                IsActive = true,
                                BrandId = tesla.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            }
                        };
                        db.Product.AddRange(products);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Lỗi khi migrate/seed dữ liệu.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
