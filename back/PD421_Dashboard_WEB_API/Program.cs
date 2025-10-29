using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PD421_Dashboard_WEB_API.BLL.Services.Auth;
using PD421_Dashboard_WEB_API.BLL.Services.Game;
using PD421_Dashboard_WEB_API.BLL.Services.Genre;
using PD421_Dashboard_WEB_API.BLL.Services.Register;
using PD421_Dashboard_WEB_API.BLL.Services.Storage;
using PD421_Dashboard_WEB_API.BLL.Settings;
using PD421_Dashboard_WEB_API.DAL;
using PD421_Dashboard_WEB_API.DAL.Entitites.Identity;
using PD421_Dashboard_WEB_API.DAL.Initializer;
using PD421_Dashboard_WEB_API.DAL.Repositories.Game;
using PD421_Dashboard_WEB_API.DAL.Repositories.Genre;
using PD421_Dashboard_WEB_API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add dbcontext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDb"));
});

// Add identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add automapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkwNTUzNjAwIiwiaWF0IjoiMTc1OTA4MjMwOSIsImFjY291bnRfaWQiOiIwMTk5OTE3OTdmZjI3OTU1OGZlZDc1MGJkMjlmOWFkOCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazY4cWtzcm1jZ3JyZmdjZmNmMDg3OGRxIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.GrZ_-FzSfYYKNOapcOC2d9OSEQlkVQNhu4-lupcU7Z3NMK7YxuXU9hPhf5I8BK-OESRDvvt_nRbj_Yf9Bav4udke8hWADgGlQUrQul0NsmotfzGt3sqW7XvbQJsf8kbWj7XIUD_pOQj-039759orXMHps2j7BILizMe2z_PS20uU5FUFKmRXfjP-tTXbBnWSEjqQYf9hmUzoyF4FhJ84tjeeG_56pOn6IX209Npkwr15jzKs1Cowf6gk27P88l8br77zrPkXKQ_9NvQh-MtRJ3J-66yyUJdB7yP8vO-C0anWZDxKLHxJA2JlH7MRxALsFDgZy1hkjmR90-x1sXM9dQ";
}, AppDomain.CurrentDomain.GetAssemblies());

// Add repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

// Add services
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
// Add settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

// CORS
string corsName = "CorsAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsName, builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(corsName);

string rootPath = app.Environment.ContentRootPath;
string storagePath = Path.Combine(rootPath, "storage");
string imagesPath = Path.Combine(storagePath, "images");

app.AddStaticFiles(app.Environment);
app.Seed();

app.Run();
