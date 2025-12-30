using Microsoft.Data.SqlClient;

namespace Discovery.Time.Tests.Data.MockData;

public static class CreateSqlException
{
    public static SqlException MakeSqlException(string connection)
    {
        SqlException exception = null;

		try
		{
			SqlConnection conn = new SqlConnection(connection);
			conn.Open();
		}
		catch (SqlException ex)
		{
			exception = ex;
		}
		return (exception);
    }
}
