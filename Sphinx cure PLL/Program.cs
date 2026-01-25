using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.BLL.Mapper.PatientMapping;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.BLL.Services.Implementations;
using Sphinx_cure_.DAL.Database;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_.DAL.Repo.Abstractions;
using Sphinx_cure_.DAL.Repo.Implementations;
using Sphinx_cure_PLL.Hubs;

namespace Sphinx_cure_PLL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Enhancement ConnectionString
            var connectionString = builder.Configuration.GetConnectionString("defaultConnection");

            builder.Services.AddDbContext<SphinxCureDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // SINGLE Identity configuration - Remove the conflicting ones!
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // DISABLE all email requirements
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Make email NOT required for users
                options.User.RequireUniqueEmail = false;

                // Simplify password rules for testing
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;

                // Optional: Adjust user settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<SphinxCureDbContext>()
            .AddDefaultTokenProviders();

            // Cookie configuration (optional - Identity has defaults)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAutoMapper(x => x.AddProfile(new PatientProfile()));
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IPatientRepo, PatientRepo>();
            builder.Services.AddScoped<IPatientService, PatientService>();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB
            });

            var app = builder.Build();

            app.MapHub<NotificationHub>("/notificationHub");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // This must come before UseAuthorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}