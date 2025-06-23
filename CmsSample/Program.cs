using CmsSample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//───────────────────────────────────────────────────────
// 1) MVC + Razor Pages (Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();              // ← Login / Register

//───────────────────────────────────────────────────────
// 2) Oracle erişim katmanları (önceden var)
builder.Services.AddSingleton<OraclePageRepository>();
builder.Services.AddSingleton<OracleSliderRepository>();
builder.Services.AddSingleton<OracleBlogRepository>();

//───────────────────────────────────────────────────────
// 3) Identity veritabanı (SQLite dosyası: identity.db)
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("IdentityDb")));

// 4) Identity servisleri
builder.Services
    .AddDefaultIdentity<IdentityUser>(opt => opt.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

//───────────────────────────────────────────────────────
// Ortak middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();      // ← Kimlik doğrulama
app.UseAuthorization();

// Razor Pages (Identity UI) + MVC rotaları
app.MapRazorPages();          // ← /Identity/Account/Login vb.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Opsiyonel Oracle debug endpoint
app.MapGet("/debug/db", async (IConfiguration cfg) =>
{
    var connStr = cfg.GetConnectionString("OracleDb");
    await using var conn = new Oracle.ManagedDataAccess.Client.OracleConnection(connStr);
    await conn.OpenAsync();
    return conn.State.ToString();     // "Open" beklenir
});

app.Run();