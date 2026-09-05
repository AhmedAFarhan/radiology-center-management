using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Infrastructure.Persistence;
using RadiologyCenter.Inventory.Infrastructure.Repositories;

namespace RadiologyCenter.Inventory.Infrastructure;

public static class InventoryInfrastructureRegistration
{
    public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InventoryDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IStockBatchRepository, StockBatchRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();

        return services;
    }
}
