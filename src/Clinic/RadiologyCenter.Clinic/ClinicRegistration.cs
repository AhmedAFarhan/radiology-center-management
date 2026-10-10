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
using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Clinic.Localization;
using RadiologyCenter.Clinic.Services;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Clinic.Security;

namespace RadiologyCenter.Clinic;

public static class ClinicRegistration
{
public static IServiceCollection AddClinicModules(this IServiceCollection services, IConfiguration configuration)
{
services.AddApplication();
services.AddInfrastructure(configuration);
services.AddIdentityApplication();
services.AddIdentityInfrastructure(configuration);
services.AddPatientsApplication();
services.AddPatientsInfrastructure(configuration);
services.AddInventoryApplication();
services.AddInventoryInfrastructure(configuration);
services.AddExaminationsApplication();
services.AddExaminationsInfrastructure(configuration);
services.AddCatalogApplication();
services.AddCatalogInfrastructure(configuration);
services.AddResourceManagementApplication();
services.AddResourceManagementInfrastructure(configuration);
services.AddPayrollApplication();
services.AddPayrollInfrastructure(configuration);
services.AddReportsApplication();
services.AddReportsInfrastructure(configuration);
services.AddInsuranceApplication();
services.AddInsuranceInfrastructure(configuration);
services.AddCashApplication();
services.AddCashInfrastructure(configuration);
services.AddNotificationApplication();
services.AddNotificationInfrastructure(configuration);
services.AddScoped<IItemSnapshotResolver, ItemSnapshotResolver>();
services.AddScoped<IBookingResourceDirectory, BookingResourceDirectory>();
services.AddScoped<IPacsStudyVerifier, PacsStudyVerifier>();
services.AddScoped<IExaminationFeeResolver, ExaminationFeeResolver>();
services.AddScoped<IReferralFeeStatementResolver, ReferralFeeStatementResolver>();
services.AddScoped<IReferralFeeStatementCalculator, ReferralFeeStatementCalculator>();
services.AddScoped<IAncillaryDirectory, AncillaryDirectory>();
services.AddScoped<RadiologyCenter.Examinations.Application.Abstractions.IExaminationTypeDirectory, ExaminationTypeInfoDirectory>();
services.AddScoped<IProfitSourceResolver, ProfitSourceResolver>();
services.AddScoped<IReportDirectory, ReportDirectory>();
services.AddScoped<IInsuranceDirectory, InsuranceDirectory>();
services.AddScoped<ICashDirectory, CashDirectory>();
services.AddScoped<IPaymentCashEntryRecorder, PaymentCashEntryRecorder>();
services.AddScoped<IAnalyticsReportService, AnalyticsReportService>();
services.AddScoped<IAnalyticsPdfService, AnalyticsPdfService>();
services.AddScoped<IInsuranceAnalyticsDataSource, InsuranceAnalyticsDataSource>();
services.AddScoped<ICashFlowDataSource, CashFlowDataSource>();
services.AddSingleton<RadiologyCenter.Clinic.Services.GlobalSearch.GlobalSearchService>();

services.AddSingleton<ITranslator, JsonTranslator>();
services.AddScoped<ClinicCurrentUser>();
services.AddSingleton<ClinicPrincipalAccessor>();
services.AddSingleton<ClinicSessionGuard>();
services.Replace(ServiceDescriptor.Scoped<IDomainEventDispatcher, SqlDomainEventJournal>());
services.Replace(ServiceDescriptor.Scoped<IInsuranceDocumentStorage, SqlInsuranceDocumentStorage>());
services.Replace(ServiceDescriptor.Scoped<ICurrentUser>(sp => sp.GetRequiredService<ClinicCurrentUser>()));
services.AddScoped<DatabasePrincipalReader>();
services.AddScoped<SafeIdentityReader>();
if (configuration.GetValue<bool>("DatabaseAuthorization:Required"))
    services.AddScoped<RadiologyCenter.Identity.Application.Abstractions.IIdentityAccessProvisioner, SqlIdentityAccessProvisioner>();
else
    services.AddScoped<RadiologyCenter.Identity.Application.Abstractions.IIdentityAccessProvisioner, RadiologyCenter.Identity.Application.Abstractions.NoIdentityAccessProvisioner>();
services.AddSingleton<ClinicDispatcher>();
services.AddHttpClient("Pacs", client => client.Timeout = TimeSpan.FromSeconds(30))
    .ConfigurePrimaryHttpMessageHandler(()=>new HttpClientHandler { AllowAutoRedirect=false });
return services;
}
}
