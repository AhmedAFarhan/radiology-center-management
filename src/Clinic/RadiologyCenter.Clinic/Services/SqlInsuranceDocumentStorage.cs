using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Clinic.Services;

public sealed class SqlInsuranceDocumentStorage(InsuranceDbContext context) : IInsuranceDocumentStorage
{
    public string RootPath=>"SQL Server / Insurance.DocumentContents";
    public async Task<string> SaveAsync(string directory,string fileName,Stream content,CancellationToken ct=default)
    {
        using var memory=new MemoryStream(); var buffer=new byte[81920]; int count;
        while((count=await content.ReadAsync(buffer,ct))>0)
        {
            if(memory.Length+count>20*1024*1024) throw new InvalidOperationException("Documents must be no larger than 20 MB.");
            await memory.WriteAsync(buffer.AsMemory(0,count),ct);
        }
        var id=Guid.NewGuid();
        await using var command=context.Database.GetDbConnection().CreateCommand();
        command.Transaction=context.Database.CurrentTransaction?.GetDbTransaction()
            ?? throw new InvalidOperationException("Document content and metadata require one database transaction.");
        command.CommandText="INSERT [Insurance].[DocumentContents](Id,Content) VALUES (@id,@content)";
        var key=command.CreateParameter(); key.ParameterName="@id"; key.Value=id; command.Parameters.Add(key);
        var data=command.CreateParameter(); data.ParameterName="@content"; data.DbType=DbType.Binary; data.Value=memory.ToArray(); command.Parameters.Add(data);
        await command.ExecuteNonQueryAsync(ct); return "db:"+id.ToString("D");
    }
    public async Task<Stream> OpenAsync(string path,CancellationToken ct=default)
    {
        var id=Id(path); await context.Database.OpenConnectionAsync(ct);
        try
        {
            await using var command=context.Database.GetDbConnection().CreateCommand();
            command.CommandText="SELECT Content FROM [Insurance].[DocumentContents] WHERE Id=@id";
            var parameter=command.CreateParameter(); parameter.ParameterName="@id"; parameter.Value=id; command.Parameters.Add(parameter);
            return await command.ExecuteScalarAsync(ct) is byte[] bytes ? new MemoryStream(bytes,writable:false) : throw new FileNotFoundException("The shared document was not found.");
        }
        finally { await context.Database.CloseConnectionAsync(); }
    }
    public bool Exists(string path)
    {
        var id=Id(path); context.Database.OpenConnection();
        try
        {
            using var command=context.Database.GetDbConnection().CreateCommand();
            command.CommandText="SELECT COUNT(*) FROM [Insurance].[DocumentContents] WHERE Id=@id";
            var p=command.CreateParameter(); p.ParameterName="@id"; p.Value=id; command.Parameters.Add(p);
            return (int)command.ExecuteScalar()!>0;
        }
        finally { context.Database.CloseConnection(); }
    }
    // Upload failure already rolls back its content row with its metadata. Soft deletion retains both for auditing.
    public void Delete(string path) { _=Id(path); }
    private static Guid Id(string path)=>path.StartsWith("db:",StringComparison.Ordinal) && Guid.TryParse(path[3..],out var id)
        ? id : throw new InvalidOperationException("This legacy document must be imported during admin-PC maintenance.");
}
