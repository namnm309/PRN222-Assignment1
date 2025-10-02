using BusinessLayer.Services;
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

            // Đăng ký Repository & Service
            builder.Services.AddScoped<IDealerRepository, DealerRepository>();
            builder.Services.AddScoped<IDealerService, DealerService>();

            // Đăng ký Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dealer API V1");
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
