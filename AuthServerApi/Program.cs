using AuthServerApi.Data;
using AuthServerApi.Model;
using AuthServerApi.Service.Authenticators;
using AuthServerApi.Service.RefreshTokenRepositories;
using AuthServerApi.Service.TokenGenerator;
using AuthServerApi.Service.TokenValidator;
using AuthServerApi.Service.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var db_host_url = Environment.GetEnvironmentVariable("DB_HOST_URL");
var db_host_port = Environment.GetEnvironmentVariable("DB_HOST_PORT");
var db_name = Environment.GetEnvironmentVariable("DB_NAME");
var db_user = Environment.GetEnvironmentVariable("DB_USER");
var db_password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = builder.Configuration.GetConnectionString("default-connection");

if (db_host_url != null && !string.IsNullOrEmpty(db_host_url)) {
    connectionString = $"Server={db_host_url},{db_host_port};Database={db_name};User={db_user};password={db_password};Trusted_Connection=False;TrustServerCertificate=True;";
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication"));
//builder.Services.AddSingleton<AuthenticationConfiguration>();

builder.Services.AddDbContext<AuthenticationDbContext>(options=>
options.UseSqlServer(connectionString, 
sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

builder.Services.AddSingleton<AccessTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenValidator>();
//builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();
builder.Services.AddScoped<Authenticator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Authentication:AccessTokenSecret").Value)),
            ValidIssuer = builder.Configuration.GetSection("Authentication:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Authentication:Audience").Value,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
var app = builder.Build();

    var service = (IServiceScopeFactory)app.Services.GetService(typeof(IServiceScopeFactory));
    using (var db = service.CreateScope().ServiceProvider.GetService<AuthenticationDbContext>())
    {
        db.Database.Migrate();
    }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
