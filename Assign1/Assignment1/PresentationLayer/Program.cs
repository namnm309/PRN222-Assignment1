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
            builder.Services.AddHttpContextAccessor();

            // Cấu hình DbContext (đăng ký DI trước khi build app)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository và Services
            builder.Services.AddScoped<IAuthen, Authen>();
            builder.Services.AddScoped<IAuthenService, AuthenService>();

            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<ITestDriveRepository, TestDriveRepository>();
            builder.Services.AddScoped<ITestDriveService, TestDriveService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            
            builder.Services.AddScoped<IDealerRepository, DealerRepository>();
            builder.Services.AddScoped<IDealerService, DealerService>();
            
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            
            builder.Services.AddScoped<IDealerContractRepository, DealerContractRepository>();
            builder.Services.AddScoped<IDealerContractService, DealerContractService>();
            
            // Đăng ký EVM Repository và Services
            builder.Services.AddScoped<IEVMRepository, EVMRepository>();
            builder.Services.AddScoped<IEVMReportService, EVMReportService>();

            // Đăng ký Inventory Management Repository và Services
            builder.Services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();
            builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();

            // Đăng ký Pricing Management Repository và Services
            builder.Services.AddScoped<IPricingManagementRepository, PricingManagementRepository>();
            builder.Services.AddScoped<IPricingManagementService, PricingManagementService>();


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

                    // Seed Admin user nếu chưa có
                    if (!db.Users.Any(u => u.Role == DataAccessLayer.Enum.UserRole.Admin))
                    {
                        var adminUser = new DataAccessLayer.Entities.Users
                        {
                            Id = Guid.NewGuid(),
                            FullName = "Administrator",
                            Email = "admin@vinfast.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                            PhoneNumber = "0123456789",
                            Address = "Hà Nội, Việt Nam",
                            Role = DataAccessLayer.Enum.UserRole.Admin,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Users.Add(adminUser);
                        db.SaveChanges();
                    }

                    // Seed EVM Staff user nếu chưa có
                    if (!db.Users.Any(u => u.Role == DataAccessLayer.Enum.UserRole.EVMStaff))
                    {
                        var evmStaffUser = new DataAccessLayer.Entities.Users
                        {
                            Id = Guid.NewGuid(),
                            FullName = "EVM Staff",
                            Email = "evm@vinfast.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("evm123"),
                            PhoneNumber = "0987654321",
                            Address = "TP.HCM, Việt Nam",
                            Role = DataAccessLayer.Enum.UserRole.EVMStaff,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Users.Add(evmStaffUser);
                        db.SaveChanges();
                    }

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

                    // Seed một Dealer mặc định nếu chưa có TRƯỚC
                    Guid defaultDealerId = Guid.Empty;
                    if (!db.Dealer.Any())
                    {
                        var defaultDealer = new DataAccessLayer.Entities.Dealer
                        {
                            Id = Guid.NewGuid(),
                            Name = "VinFast Đại Lý Hà Nội",
                            Phone = "024-1234-5678",
                            Address = "123 Phố Huế, Hai Bà Trưng",
                            City = "Hà Nội",
                            Province = "Hà Nội",
                            RegionId = null, // chưa cấu hình vùng
                            DealerCode = "DL-HN-001",
                            ContactPerson = "Nguyễn Văn C",
                            Email = "dl-hn-001@vinfast.com",
                            LicenseNumber = "DLHN001",
                            CreditLimit = 1_000_000_000m,
                            OutstandingDebt = 0m,
                            Status = "Active",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Dealer.Add(defaultDealer);
                        db.SaveChanges();
                        defaultDealerId = defaultDealer.Id;
                    }
                    else
                    {
                        defaultDealerId = db.Dealer.First().Id;
                    }

                    // Seed Dealer users nếu chưa có
                    if (!db.Users.Any(u => u.Role == DataAccessLayer.Enum.UserRole.DealerManager))
                    {
                        var dealerManager = new DataAccessLayer.Entities.Users
                        {
                            Id = Guid.NewGuid(),
                            FullName = "Nguyễn Văn A",
                            Email = "dealer.manager@vinfast.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("dealer123"),
                            PhoneNumber = "0123456788",
                            Address = "Hà Nội, Việt Nam",
                            Role = DataAccessLayer.Enum.UserRole.DealerManager,
                            DealerId = defaultDealerId, // Gán vào dealer mặc định
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Users.Add(dealerManager);
                        db.SaveChanges();
                    }

                    if (!db.Users.Any(u => u.Role == DataAccessLayer.Enum.UserRole.DealerStaff))
                    {
                        var dealerStaff = new DataAccessLayer.Entities.Users
                        {
                            Id = Guid.NewGuid(),
                            FullName = "Trần Thị B",
                            Email = "dealer.staff@vinfast.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
                            PhoneNumber = "0987654320",
                            Address = "TP.HCM, Việt Nam",
                            Role = DataAccessLayer.Enum.UserRole.DealerStaff,
                            DealerId = defaultDealerId, // Gán vào dealer mặc định
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Users.Add(dealerStaff);
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
