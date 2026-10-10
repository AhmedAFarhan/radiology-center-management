using System.Text.Json;
using Microsoft.Data.SqlClient;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Clinic.Dispatching;

namespace RadiologyCenter.Clinic.Security;

/// <summary>Metadata projections do not expose password hashes, sessions or refresh tokens.</summary>
public sealed class SafeIdentityReader(IConfiguration configuration)
{
    public bool Enabled=>configuration.GetValue<bool>("DatabaseAuthorization:Required");
    public async Task<IReadOnlyList<T>> ReadAsync<T>(string directory,CancellationToken ct)
    {
        if(directory is not ("User" or "Role")) throw new ArgumentException(nameof(directory));
        await using var connection=new SqlConnection(configuration.GetConnectionString("DefaultConnection")); await connection.OpenAsync(ct);
        await using var command=connection.CreateCommand(); command.CommandText="Identity.Read"+directory+"Directory";
        command.CommandType=System.Data.CommandType.StoredProcedure;
        return JsonSerializer.Deserialize<List<T>>((string)(await command.ExecuteScalarAsync(ct))!,ClinicDispatcher.JsonOptions)??[];
    }
    public static PagedResult<T> Page<T>(IReadOnlyList<T> items,QueryRequest request) where T:class
    {
        var spec=new DynamicSpecification<T>(FilterExpressionBuilder.Build<T>(request.Filters));
        if(SearchExpressionBuilder.Build<T>(request.SearchTerm,request.SearchFields) is {} search) spec.AddCriteria(search);
        if(SortExpressionBuilder.TryBuildSelector<T>(request.SortBy,out var sort))
        { if(request.SortDescending) spec.ApplyOrderByDescending(sort); else spec.ApplyOrderBy(sort); }
        var query=SpecificationEvaluator<T>.GetQuery(items.AsQueryable(),spec); var count=query.Count();
        return new(query.Skip((request.Pagination.PageNumber-1)*request.Pagination.PageSize).Take(request.Pagination.PageSize).ToArray(),count,request.Pagination.PageNumber,request.Pagination.PageSize);
    }
}
