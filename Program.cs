using Microsoft.EntityFrameworkCore;
using Rh.Data;
using Rh.Models;

var builder = WebApplication.CreateBuilder(args);

// Connexion à MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est manquante.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Controllers avec Views
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.ResponsablesRH.Any())
    {
        string motDePasseClair = "admin123";                  
        string hash = BCrypt.Net.BCrypt.HashPassword(motDePasseClair); 

        var adminUser = new ResponsableRH
        {
            Email = "admin@rh.com",
            MotDePasseHash = hash, 
            EstActif = true,
            DateCreation = DateTime.Now
        };

        context.ResponsablesRH.Add(adminUser);
        context.SaveChanges();
    }
}
app.Run();
