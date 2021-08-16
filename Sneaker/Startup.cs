using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sneaker.Data;
using Sneaker.Data.DbInit;
using Sneaker.Models;
using Sneaker.Repository;
using Sneaker.Repository.Interface;
using System;
using System.Collections.Immutable;

namespace Sneaker
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlServerDocker")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAdminRepo, AdminRepo>();
            services.AddScoped<ITrademarkRepo, TrademarkRepo>();
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IFeedbackProductRepo, FeedbackProductRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICartRepo, CartRepo>();
            services.AddScoped(c => CartItem.GetCart(c));

            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddDistributedMemoryCache();
            services.AddSession(cfg =>
            {
                cfg.Cookie.Name = "Sneaker";
                cfg.IdleTimeout = new TimeSpan(0, 30, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            dbInitializer.Initializer();
            
            
        }
    }
}
