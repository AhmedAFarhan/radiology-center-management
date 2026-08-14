using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.BuildingBlocks.Infrastructure;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Identity.Application;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Identity.Infrastructure.Persistence.Seed;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Localhost.Filters;
using RadiologyCenter.Localhost.Middleware;
using RadiologyCenter.Patients.Application;
using RadiologyCenter.Patients.Infrastructure;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.Inventory.Application;
using RadiologyCenter.Inventory.Infrastructure;
using RadiologyCenter.Inventory.Infrastructure.Persistence;
using RadiologyCenter.Examinations.Application;
using RadiologyCenter.Examinations.Infrastructure;
using RadiologyCenter.Examinations.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Application;
using RadiologyCenter.ResourceManagement.Infrastructure;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Application;
using RadiologyCenter.Payroll.Infrastructure;
using RadiologyCenter.Payroll.Infrastructure.Persistence;
using RadiologyCenter.Reports.Application;
using RadiologyCenter.Reports.Infrastructure;
using RadiologyCenter.Reports.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Application;
using RadiologyCenter.Insurance.Infrastructure;
using RadiologyCenter.Insurance.Infrastructure.Persistence;
using RadiologyCenter.Cash.Application;
using RadiologyCenter.Cash.Infrastructure;
using RadiologyCenter.Cash.Infrastructure.Persistence;
using RadiologyCenter.Notification.Application;
using RadiologyCenter.Notification.Infrastructure;
using RadiologyCenter.Notification.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalResponseFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT access token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Host.ConfigureWolverine(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddApplication();
builder.Services.AddMapster();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddPatientsApplication();
builder.Services.AddPatientsInfrastructure(builder.Configuration);
builder.Services.AddInventoryApplication();
builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddExaminationsApplication();
builder.Services.AddExaminationsInfrastructure(builder.Configuration);
builder.Services.AddResourceManagementApplication();
builder.Services.AddResourceManagementInfrastructure(builder.Configuration);
builder.Services.AddPayrollApplication();
builder.Services.AddPayrollInfrastructure(builder.Configuration);
builder.Services.AddReportsApplication();
builder.Services.AddReportsInfrastructure(builder.Configuration);
builder.Services.AddInsuranceApplication();
builder.Services.AddInsuranceInfrastructure(builder.Configuration);
builder.Services.AddCashApplication();
builder.Services.AddCashInfrastructure(builder.Configuration);
builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);
builder.Services.AddScoped<IItemSnapshotResolver, ItemSnapshotResolver>();
builder.Services.AddScoped<IExaminationFeeResolver, ExaminationFeeResolver>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IPayrollStaffDirectory, PayrollStaffDirectory>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IExaminationTypeDirectory, ExaminationTypeDirectory>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IReferralDoctorDirectory, ReferralDoctorDirectory>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IExamFeeIncomeResolver, PayrollFeeIncomeResolver>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IStaffLeaveResolver, StaffLeaveDaysResolver>();
builder.Services.AddScoped<RadiologyCenter.Payroll.Application.Abstractions.IStaffWorkHoursResolver, StaffWorkHoursResolver>();
builder.Services.AddScoped<RadiologyCenter.Examinations.Application.Abstractions.IAncillaryDirectory, AncillaryDirectory>();
builder.Services.AddScoped<RadiologyCenter.Examinations.Application.Abstractions.IProfitSourceResolver, ProfitSourceResolver>();
builder.Services.AddScoped<RadiologyCenter.Reports.Application.Abstractions.IReportDirectory, ReportDirectory>();
builder.Services.AddScoped<RadiologyCenter.Insurance.Application.Abstractions.IInsuranceDirectory, InsuranceDirectory>();
builder.Services.AddScoped<RadiologyCenter.Cash.Application.Abstractions.ICashDirectory, CashDirectory>();
builder.Services.AddScoped<IPaymentCashEntryRecorder, PaymentCashEntryRecorder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContextTypes = new[]
    {
        typeof(AppDbContext),
        typeof(IdentityDbContext),
        typeof(PatientsDbContext),
        typeof(InventoryDbContext),
        typeof(ExaminationsDbContext),
        typeof(ResourceManagementDbContext),
        typeof(PayrollDbContext),
        typeof(ReportsDbContext),
        typeof(InsuranceDbContext),
        typeof(CashDbContext),
        typeof(NotificationDbContext)
    };

    foreach (var dbContextType in dbContextTypes)
    {
        var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
        dbContext.Database.Migrate();
    }

    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await IdentityDbSeeder.SeedAsync(
        identityDb,
        scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>());
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
