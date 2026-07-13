using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuramos la conexión a la base de datos de Kovak
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaConexion")));

// SESIONES: Registramos la memoria en el contenedor de servicios
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Expira tras 1 hora de inactividad
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// SESIONES: Activamos el uso de sesiones en la tubería HTTP (Debe ir antes de MapControllerRoute)
app.UseSession();

app.UseStaticFiles();

// 1. RUTA POR DEFECTO PARA EL PANEL DE CONTROL (Prioridad para que funcionen los controladores)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 2. RUTA DINÁMICA PARA LOS CATÁLOGOS PÚBLICOS CON PREFIJO /t/
app.MapControllerRoute(
    name: "catalogo_tienda",
    pattern: "t/{slug}",
    defaults: new { controller = "Catalogo", action = "Index" });

app.Run();