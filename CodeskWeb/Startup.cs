using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodeskWeb.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using System;
using CodeskLibrary.Connections;
using CodeskWeb.Services;
using System.IO;

namespace CodeskWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.IsEssential = true;
                    options.Cookie.Name = "Codesk.AuthCookieAspNetCore";
                    options.LoginPath = "/Users/Account/SignIn";
                    options.LogoutPath = "/Users/Account/Logout";
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

            if (Environment.IsDevelopment())
            {
                services.AddSignalR(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                }).AddMessagePackProtocol();
            }
            else
            {
                services.AddSignalR(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                }).AddAzureSignalR(Configuration.GetConnectionString("CodeskSignalR")).AddMessagePackProtocol();
            }

            services.AddHttpClient<ICodeExecutionService, CodeExecutionService>(options => options.BaseAddress = new Uri(Configuration["CodeExecution:BaseAddress"]));

            services.AddHttpClient<IEmailService, EmailService>(options => options.BaseAddress = new Uri(Configuration["EmailService:BaseAddress"]));

            services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter()));

            DbConnection.Configuration = Configuration;

            if (Environment.IsProduction())
            {
                var dir1 = Path.Join(Environment.WebRootPath, "assets", "session", "files");
                var dir2 = Path.Join(Environment.WebRootPath, "assets", "session", "temp");

                Directory.CreateDirectory(dir1);
                Directory.CreateDirectory(dir2);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/InternalServerError");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    ctx.Items["originalPath"] = ctx.Request.Path.Value;
                    ctx.Request.Path = "/Error/PageNotFound";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area}/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<SessionHub>("/sessionHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
            });
        }
    }
}