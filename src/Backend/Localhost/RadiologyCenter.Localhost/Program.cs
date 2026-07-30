using Wolverine;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.BuildingBlocks.Infrastructure;
using RadiologyCenter.Idnetity.Application;
using RadiologyCenter.Idnetity.Infrastructure;
using RadiologyCenter.Localhost.Filters;
using RadiologyCenter.Localhost.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalResponseFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Host.UseWolverine();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
