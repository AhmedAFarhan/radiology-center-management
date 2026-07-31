using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.BuildingBlocks.Infrastructure;
using RadiologyCenter.Idnetity.Application;
using RadiologyCenter.Idnetity.Domain.Entities;
using RadiologyCenter.Idnetity.Infrastructure;
using RadiologyCenter.Idnetity.Infrastructure.Persistence;
using RadiologyCenter.Idnetity.Infrastructure.Persistence.Seed;
using RadiologyCenter.Localhost.Filters;
using RadiologyCenter.Localhost.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalResponseFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureWolverine();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    identityDb.Database.Migrate();
    await IdentityDbSeeder.SeedAsync(
        identityDb,
        scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>());
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
