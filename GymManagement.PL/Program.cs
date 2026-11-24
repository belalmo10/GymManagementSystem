using GymManagement.BLL;
using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Classes;
using GymManagement.DAL.Repositorities.Intefaces;
using GymManagement.DAL.Repositorities.Interfaces;
using GymManagement.PL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace GymManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberService , MemberService>();
            builder.Services.AddScoped<IPlanService , PlanService>();
            builder.Services.AddScoped<ITrainerService , TrainerService>();
            builder.Services.AddScoped<ISessionService , SessionService>();
            builder.Services.AddScoped<ISessionRepository , SessionRepository>();
            builder.Services.AddScoped<IAnalyticsService , AnalyticsService>();
            builder.Services.AddScoped<IAttachmentService , AttachmentService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            builder.Services.AddIdentity<ApplicationUser , IdentityRole>(Config =>
            {
                //Config.Password.RequiredLength = 6;
                //Config.Password.RequireLowercase = true;
                //Config.Password.RequireUppercase = true;
                Config.User.RequireUniqueEmail = true;
                Config.Lockout.MaxFailedAccessAttempts = 5;
                Config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

            }).AddEntityFrameworkStores<GymDbContext>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Redirect UnAuthenticated Users (401)
                options.LoginPath = "/Account/Login";
                // Redirect Forbidden users (403)
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            await app.MigrateAndSeedDatabaseAsync();


            // Configure the HTTP request pipeline.
            if ( !app.Environment.IsDevelopment() )
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default" ,
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
