using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using TrackFinder.Context;
using TrackFinder.Filters;
using TrackFinder.Models.UserModels;
using TrackFinder.Providers.Common.EmailService;
using TrackFinder.Providers.Private.AuthProviders;
using TrackFinder.Services;
using TrackFinder.Services.AuthServices.Helpers;
using TrackFinder.Services.AuthServices.Implementations;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.Services.UserProfileServices;
namespace TrackFinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection", "Invalid Connection String");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Configure the Identity application cookie (used by SignInManager)
            // instead of adding a separate cookie scheme that SignInManager ignores.
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Login/Logout";
                options.AccessDeniedPath = "/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = "tf_auth";
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ProfilePictureFilter>();
            });
            builder.Services.AddScoped<IEncryptPasswordProvider, EncryptPasswordProvider>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUserFunctions, UserFunctions>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<AssessmentService>();
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "text/html; charset=utf-8";
                    await context.HttpContext.Response.WriteAsync(
                        "<!DOCTYPE html><html><head><meta charset=\"utf-8\"/><title>Too Many Requests</title>" +
                        "<style>body{font-family:sans-serif;display:flex;justify-content:center;align-items:center;height:100vh;margin:0;background:#f5f5f5;}" +
                        ".card{background:#fff;padding:40px;border-radius:12px;box-shadow:0 2px 12px rgba(0,0,0,.1);text-align:center;}" +
                        "h1{color:#dc2626;margin:0 0 8px;font-size:28px;}p{color:#6b7280;margin:0;}</style></head><body>" +
                        "<div class=\"card\"><h1>429 — Too Many Requests</h1><p>Too many requests. Please wait 10 minutes and try again.</p></div>" +
                        "</body></html>",
                        cancellationToken);
                };

                options.AddFixedWindowLimiter("Auth", auth =>
                {
                    auth.PermitLimit = 10;
                    auth.Window = TimeSpan.FromMinutes(1);
                    auth.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    auth.QueueLimit = 0;
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter<string>(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 5
                        }));
            });
            var app = builder.Build();

            app.UseMiddleware<TrackFinder.Middleware.ExceptionHandlingMiddleware>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRateLimiter();
            app.UseSession();
            app.UseAuthentication();
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