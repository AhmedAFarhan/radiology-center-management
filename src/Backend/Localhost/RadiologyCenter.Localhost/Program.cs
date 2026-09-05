using Microsoft.OpenApi.Models;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Infrastructure;
using RadiologyCenter.BuildingBlocks.Infrastructure.Messaging;
using RadiologyCenter.Cash.Application;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Infrastructure;
using RadiologyCenter.Catalog.Application;
using RadiologyCenter.Catalog.Infrastructure;
using RadiologyCenter.Examinations.Application;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Examinations.Application.Reports;
using RadiologyCenter.Examinations.Infrastructure;
using RadiologyCenter.Examinations.Infrastructure.Services;
using RadiologyCenter.Identity.Application;
using RadiologyCenter.Identity.Infrastructure;
using RadiologyCenter.Insurance.Application;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Infrastructure;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Localhost.Filters;
using RadiologyCenter.Localhost.Localization;
using RadiologyCenter.Localhost.Middleware;
using RadiologyCenter.Notification.Application;
using RadiologyCenter.Notification.Infrastructure;
using RadiologyCenter.Patients.Application;
using RadiologyCenter.Patients.Infrastructure;
using RadiologyCenter.Payroll.Application;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Services;
using RadiologyCenter.Payroll.Infrastructure;
using RadiologyCenter.Reports.Application;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Infrastructure;
using RadiologyCenter.ResourceManagement.Application;
using RadiologyCenter.ResourceManagement.Infrastructure;
using RadiologyCenter.Inventory.Application;
using RadiologyCenter.Inventory.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalResponseFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin =>
                  allowedOrigins.Length == 0 ||
                  allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase));
    });
});

builder.Services.AddSingleton<ITranslator, JsonTranslator>();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new System.Globalization.CultureInfo("en"), new System.Globalization.CultureInfo("ar") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Radiology Center API";
    options.Version = "v1";
});
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

builder.Host.ConfigureWolverine(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddPatientsApplication();
builder.Services.AddPatientsInfrastructure(builder.Configuration);
builder.Services.AddInventoryApplication();
builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddExaminationsApplication();
builder.Services.AddExaminationsInfrastructure(builder.Configuration);
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);
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
builder.Services.AddScoped<IReferralFeeStatementResolver, ReferralFeeStatementResolver>();
builder.Services.AddScoped<IReferralFeeStatementCalculator, ReferralFeeStatementCalculator>();
builder.Services.AddScoped<IAncillaryDirectory, AncillaryDirectory>();
builder.Services.AddScoped<RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory, ExaminationTypeInfoDirectory>();
builder.Services.AddScoped<IProfitSourceResolver, ProfitSourceResolver>();
builder.Services.AddScoped<IReportDirectory, ReportDirectory>();
builder.Services.AddScoped<IInsuranceDirectory, InsuranceDirectory>();
builder.Services.AddScoped<ICashDirectory, CashDirectory>();
builder.Services.AddScoped<IPaymentCashEntryRecorder, PaymentCashEntryRecorder>();
builder.Services.AddScoped<IAnalyticsReportService, AnalyticsReportService>();
builder.Services.AddScoped<IAnalyticsPdfService, AnalyticsPdfService>();
builder.Services.AddScoped<IInsuranceAnalyticsDataSource, InsuranceAnalyticsDataSource>();
builder.Services.AddScoped<ICashFlowDataSource, CashFlowDataSource>();
builder.Services.AddSingleton<RadiologyCenter.Localhost.Services.GlobalSearch.GlobalSearchService>();

var app = builder.Build();

Translator.Current = app.Services.GetRequiredService<ITranslator>();

await app.Services.MigrateAndSeedAsync(Path.Combine(app.Environment.ContentRootPath, "Resources"));

app.UseRequestLocalization();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
app.MapHub<RadiologyCenter.BuildingBlocks.Infrastructure.RealTime.NotificationHub>("/hubs/notifications").AllowAnonymous();

app.Run();
