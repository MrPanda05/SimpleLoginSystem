using Microsoft.EntityFrameworkCore;
using SimpleLoginSystem;
using SimpleLoginSystem.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EcommerceDB>(opt => opt.UseInMemoryDatabase("Ecommerce"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

RouteGroupBuilder userItems = app.MapGroup("/users");
UserUserEndPoints.MapUserEndpoints(userItems);


app.Run();
