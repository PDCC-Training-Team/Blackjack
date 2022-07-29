using System.Data;
using System.Data.SqlClient;

namespace Blackjack.Web.App.Data.Repos;

public abstract class BaseRepo
{
    private readonly BaseDataService _dataService;

    protected BaseRepo(BaseDataService dataService)
    {
        _dataService = dataService;
    }

    protected virtual async Task Create(string sproc, List<SqlParameter> sprocParams)
    {
            using SqlConnection conn = _dataService.GetConnection();
            await conn.OpenAsync();
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sproc;
            cmd.Parameters.AddRange(sprocParams.ToArray());
            await cmd.ExecuteNonQueryAsync();
            await conn.CloseAsync();
    }

    protected virtual async Task<DataTable> Get(string sproc, List<SqlParameter> sprocParams)
    {
        using SqlConnection conn = _dataService.GetConnection();
        await conn.OpenAsync();
        using SqlCommand cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = sproc;
        cmd.Parameters.AddRange(sprocParams.ToArray());
        using SqlDataReader queryResults = await cmd.ExecuteReaderAsync();
        DataTable resultTable = new DataTable();
        resultTable.Load(queryResults);
        await conn.CloseAsync();
        return resultTable;
    }

    protected virtual async Task<DataTable> Update(string sproc, List<SqlParameter> sprocParams)
    {
        using SqlConnection conn = _dataService.GetConnection();
        await conn.OpenAsync();
        using SqlCommand cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = sproc;
        cmd.Parameters.AddRange(sprocParams.ToArray());
        using SqlDataReader queryResults = await cmd.ExecuteReaderAsync();
        DataTable resultTable = new DataTable();
        resultTable.Load(queryResults);
        await conn.CloseAsync();
        return resultTable;
    }
}
