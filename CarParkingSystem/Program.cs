using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.EmailSender;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)

    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Login/Login";
    options.LogoutPath = $"/Login/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var LoggerFactory = service.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = service.GetRequiredService<ApplicationDbContext>();

        //Create database on the first time run on the database apply only migration first then database create auto.

        if (context.Database.CanConnect())
        {
            Console.WriteLine("The database already exists.");
        }
        else
        {
            context.Database.Migrate();
        }

        var UserManeger = service.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManeger = service.GetRequiredService<RoleManager<IdentityRole>>();

        //Add defafule Three roles
        await ContextSeed.seedRolesAsync(service.GetRequiredService<RoleManager<IdentityRole>>());

        //Add default Admin who manage web application

        await ContextSeed.SeedSuperAdminAsync(UserManeger, roleManeger);


    }
    catch (Exception ex)
    {
        var logger = LoggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
