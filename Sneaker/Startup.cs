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
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sneaker.SocketServices;

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
            services.AddScoped<IFeedbackProductRepo, FeedbackProductRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICartRepo, CartRepo>();
            services.AddScoped<IInvoiceRepo, InvoiceRepo>();
            services.AddScoped<IItemRepo, ItemRepo>();
            services.AddScoped<IChartRepo, ChartRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.CallbackPath =  "/signin-google";
            });


            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddDistributedMemoryCache();
            services.AddSession(cfg =>
            {
                cfg.Cookie.Name = "Sneaker";
                cfg.IdleTimeout = new TimeSpan(0, 30, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer, IServiceProvider serviceProvider)
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
            
            // app.UseWebSockets();
            // var wsOptions = new WebSocketOptions()
            // {
            //     KeepAliveInterval = TimeSpan.FromSeconds(120),
            //     ReceiveBufferSize = 4 * 1024
            // };
            // wsOptions.AllowedOrigins.Add("ip");
            // //
            // // app.UseWebSockets(wsOptions);
            //
            // app.UseWebSockets(wsOptions);
            // app.MapWebSocketManager("/ws", serviceProvider.GetService<ItemWebsocketHandler>());
            // // app.Use(async (context, next) =>
            // // {
            // //     if (context.Request.Path == "/send")
            // //     {
            // //         if (context.WebSockets.IsWebSocketRequest)
            // //         {
            // //             using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
            // //             {
            // //                 await Send(context, webSocket);
            // //             }
            // //         }
            // //         else
            // //         {
            // //             context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            // //         }
            // //     }
            // //     else
            // //     {
            // //         await next();
            // //     }
            // //
            // // });
            app.UseDefaultFiles();
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
        
        
        // private async Task Send(HttpContext context, WebSocket webSocket)
        // {
        //     var buffer = new byte[1024 * 4];
        //     WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //     while (!result.CloseStatus.HasValue)
        //     {
        //         await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        //
        //         result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //     }
        //     await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        // }
        
    }
}
