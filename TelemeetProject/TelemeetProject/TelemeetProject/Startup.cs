using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TelemeetProject.Hubs;
using TelemeetProject.Models;
using TelemeetProject.Models.Room;

namespace TelemeetProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IConfiguration>(Configuration);
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddSignalR();
            services.AddScoped<RoomUsersDB>();
            services.AddTransient<IRoomUserDB, RoomUsersDB>();
            services.AddTransient<IUserDB, UserDB>();
            services.AddTransient<IRoomDB, RoomDB>();
            services.AddTransient<IActivityDB, ActivityDB>();
            services.AddTransient<ITimeDB, TimeDB>();
            //Multipart
            services.Configure<FormOptions>(x =>
            {
                x.MemoryBufferThreshold = Int32.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RoomHub>("/signalServer");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
