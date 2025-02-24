using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DataServerApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication"));
//builder.Services.AddSingleton<AuthenticationConfiguration>();

AuthenticationConfiguration configuration = new AuthenticationConfiguration();
builder.Configuration.Bind("Authentication",configuration);

//SecretClient keyVaultClient = new SecretClient(
//    new Uri(builder.Configuration.GetSection("KeyVaultUri").Value),
//    new DefaultAzureCredential()
//    );
//configuration.AccessTokenSecret = keyVaultClient.GetSecret("access-token-secret").Value.Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters() {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.AccessTokenSecret)),
        ValidIssuer = configuration.Issuer,
        ValidAudience = configuration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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



