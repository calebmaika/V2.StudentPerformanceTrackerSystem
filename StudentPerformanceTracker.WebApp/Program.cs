using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Services.Authentication;
using StudentPerformanceTracker.Services.Teachers;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.Services.Students;
using StudentPerformanceTracker.Services.Curriculum;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure SQLite Database
// Connection string is in appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";  // Where to redirect if not logged in
        options.LogoutPath = "/Admin/Account/Logout";  // Logout URL
        options.AccessDeniedPath = "/Admin/Account/AccessDenied";  // Access denied URL
        options.ExpireTimeSpan = TimeSpan.FromHours(2);  // Cookie expires in 2 hours
        options.SlidingExpiration = true;  // Refresh expiration on each request
        options.Cookie.HttpOnly = true;  // Cookie not accessible via JavaScript (security)
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Only send over HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;  // CSRF protection
    });

// Register our custom services
// Scoped = new instance per request
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAdminAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();

// Add Session support (optional but useful)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Serve files from wwwroot

app.UseRouting();

app.UseAuthentication();  // IMPORTANT: Must come before UseAuthorization
app.UseAuthorization();

app.UseSession();

// Configure routes
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

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();  // Creates database if it doesn't exist
}

app.Run();


//add migration
//dotnet ef migrations add <migrationName> --project StudentPerformanceTracker.Data --startup-project StudentPerformanceTracker.WebApp

// Update database
//dotnet ef database update --project StudentPerformanceTracker.Data --startup-project StudentPerformanceTracker.WebApp