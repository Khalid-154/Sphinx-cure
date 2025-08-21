using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.BLL.Mapper.PatientMapping;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.BLL.Services.Implementations;
using Sphinx_cure_.DAL.Database;
using Sphinx_cure_.DAL.Repo.Abstractions;
using Sphinx_cure_.DAL.Repo.Implementations;

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

            builder.Services.AddAutoMapper(x => x.AddProfile(new PatientProfile()));



            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IPatientRepo, PatientRepo>();
            builder.Services.AddScoped<IPatientService, PatientService>();

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
