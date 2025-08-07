using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleLoginSystem;
using SimpleLoginSystem.Endpoints;
using SimpleLoginSystem.Services;
using SimpleLoginSystem.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TokenService>();

var secretKey = ApiSettings.GenerateSecretByte();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("common", policy => policy.RequireRole("common"));
});

builder.Services.AddAuthorization();
builder.Services.AddDbContext<EcommerceDB>(opt => opt.UseInMemoryDatabase("Ecommerce"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

await app.SeedAdminUserAsync();

RouteGroupBuilder userItems = app.MapGroup("/users");
UserUserEndPoints.MapUserEndpoints(userItems);
RouteGroupBuilder productItems = app.MapGroup("/products");
ProductsEndPoints.MapProductEndpoints(productItems);


app.Run();
