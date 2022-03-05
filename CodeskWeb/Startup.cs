using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using CodeskWeb.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using System;
using CodeskLibrary.Connections;
using CodeskWeb.Services;

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
                })
                .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                })
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = Configuration["Authentication:GitHub:ClientId"];
                    options.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                    options.CallbackPath = new PathString("/signin-github");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.SaveTokens = true;
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted).ConfigureAwait(false);
                            response.EnsureSuccessStatusCode();
                            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                            context.RunClaimActions(json.RootElement);
                        }
                    };
                })
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                });

            services.AddAutoMapper(typeof(Startup));

            services
                .AddFluentEmail(Configuration["EmailSettings:Sender"])
                .AddRazorRenderer()
                .AddSmtpSender(
                    Configuration["EmailSettings:Host"],
                    int.Parse(Configuration["EmailSettings:Port"]),
                    Configuration["EmailSettings:Username"],
                    Configuration["EmailSettings:Password"]
                );

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

            services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter()));

            DbConnection.Configuration = Configuration;
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