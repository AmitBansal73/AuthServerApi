using AuthServerApi;
using AuthServerApi.Data;
using AuthServerApi.Model;
using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
//builder.Services.AddSwaggerGen();

builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication"));
//builder.Services.AddSingleton<AuthenticationConfiguration>();

builder.Services.AddDbContext<AuthenticationDbContext>(options=>
options.UseSqlServer(connectionString, 
sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));


builder.Services.AddServiceDependencies();
builder.Services.AddAuthenticationService(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowCredentials()
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
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
