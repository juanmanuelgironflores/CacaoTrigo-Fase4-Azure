using CacaoTrigo.Data;
using CacaoTrigo.Models;
using CacaoTrigo.Repositories;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContext<CacaoTrigoContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IInsumoRepository, InsumoRepository>();
builder.Services.AddScoped<IInsumoService, InsumoService>();
builder.Services.AddScoped<IMesaRepository, MesaRepository>();
builder.Services.AddScoped<IMesaService, MesaService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
builder.Services.AddScoped<IPromocionService, PromocionService>();
builder.Services.AddScoped<IDetallePedidoRepository, DetallePedidoRepository>();
builder.Services.AddScoped<IDetallePedidoService, DetallePedidoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IImagenService, ImagenService>();

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<CacaoTrigoContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<IdentityErrorDescriberEs>()
    .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Issuer.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Audience.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key debe tener al menos 32 bytes.");
}

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.LogoutPath = "/Cuenta/Logout";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega aquí el token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = contexto =>
    {
        if (contexto.Context.Request.Path.StartsWithSegments("/uploads"))
        {
            contexto.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            contexto.Context.Response.Headers["Content-Security-Policy"] =
                "default-src 'none'; style-src 'unsafe-inline'; sandbox";
        }
    }
});

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var contexto = scope.ServiceProvider.GetRequiredService<CacaoTrigoContext>();
    using var limite = new CancellationTokenSource(TimeSpan.FromSeconds(90));

    try
    {
        logger.LogInformation(
            "Cacao y Trigo: aplicando migraciones de la base de datos...");
        await contexto.Database.MigrateAsync(limite.Token);
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Falló la migración de la base de datos; se intentará reparar el esquema.");
    }

    try
    {
        await EsquemaBaseDatos.AsegurarAsync(contexto, limite.Token);
        logger.LogInformation(
            "Cacao y Trigo: esquema de la base de datos verificado.");
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "No se pudo verificar el esquema. Revisa la cadena de conexión " +
            "'DefaultConnection' y que SQL Server LocalDB esté disponible.");
    }

    try
    {
        await SeedIdentity.InicializarAsync(scope.ServiceProvider, app.Configuration)
            .WaitAsync(limite.Token);
        logger.LogInformation(
            "Cacao y Trigo: roles y administrador listos.");
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "No se pudieron crear los roles o el administrador inicial.");
    }
}

app.Run();
