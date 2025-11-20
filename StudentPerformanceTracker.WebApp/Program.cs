using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Services.Authentication;
using StudentPerformanceTracker.Services.Teachers;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.Services.Students;
using StudentPerformanceTracker.Services.Curriculum;
using StudentPerformanceTracker.WebApp.Middleware;
using StudentPerformanceTracker.Services.Auditing;
using Serilog;
using Serilog.Events; 
using AspNetCoreRateLimit;

// Configure Serilog BEFORE building the app
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Student Performance Tracker application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllersWithViews();

    // Configure SQLite Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Configure Cookie Authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Admin/Account/Login";
            options.LogoutPath = "/Admin/Account/Logout";
            options.AccessDeniedPath = "/Admin/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });

    // Register services
    builder.Services.AddScoped<IPasswordService, PasswordService>();
    builder.Services.AddScoped<IAdminAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<ITeacherAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<ITeacherService, TeacherService>();
    builder.Services.AddScoped<ISubjectService, SubjectService>();
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<ICurriculumService, CurriculumService>();
    builder.Services.AddScoped<IAuditService, AuditService>();

    // Add Session support
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Add rate limiting
    // builder.Services.AddMemoryCache();
    // builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    // builder.Services.AddInMemoryRateLimiting();
    // builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }


    // ... in app configuration ...

    // Add security headers middleware
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // Add rate limiting
    //app.UseIpRateLimiting();

    // Add Serilog request logging
    app.UseSerilogRequestLogging();

    // Add custom exception handling middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Status code pages
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();

    // Configure routes
    app.MapControllerRoute(
        name: "admin_default",
        pattern: "Admin/{controller=Account}/{action=Login}/{id?}");

    app.MapControllerRoute(
        name: "teacher_default",
        pattern: "Teacher/{controller=Account}/{action=Login}/{id?}");

    app.MapControllerRoute(
        name: "student_default",
        pattern: "Student/{controller=Account}/{action=Login}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Apply database migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            dbContext.Database.Migrate();
            Log.Information("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


//add migration
//dotnet ef migrations add <migrationName> --project StudentPerformanceTracker.Data --startup-project StudentPerformanceTracker.WebApp

// Update database
//dotnet ef database update --project StudentPerformanceTracker.Data --startup-project StudentPerformanceTracker.WebApp