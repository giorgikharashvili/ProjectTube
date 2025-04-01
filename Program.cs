using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using ProjectTube.Data;
using ProjectTube.Models;
using ProjectTube.Repositories;


namespace ProjectTube
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
            });

            

            // Without this it will give error when implementing in controller or anywhere else
            builder.Services.AddScoped<IRepository<VideoPosting>,  VideoPostingRepository>(); 
            
            
            
            
            // if we dont add this line we wont be able to create users or create roles
            // we are telling the Entity Framework to use the IdentityUser and the Identity Role for
            // identity management this means the Entity framework will use IdentityUser and IdentityRole
            // for creating users and roles
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                           .AddRoles<IdentityRole>()
                           .AddEntityFrameworkStores<ApplicationDbContext>();


            
            
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var testUser = await userManager.FindByEmailAsync("test@example.com");
                if (testUser == null)
                {
                    testUser = new IdentityUser { UserName = "testuser", Email = "test@example.com", EmailConfirmed = true };
                    await userManager.CreateAsync(testUser, "Test@123");
                }
            }

            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await RoleSeeder.SeedRolesAsync(services);
                await UserSeeder.SeedUsersAsync(services); // what are services what are we passing to UserSeeder.SeedUserAsync function

            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.MapRazorPages();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
