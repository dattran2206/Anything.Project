using Base.DataAccess.Helpers;
using Base.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Base.DataAccess.Services
{
    public class DataBaseService : IDataBaseService
    {
        private readonly string _connectionString;

        public DataBaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") + "";
        }

        public async Task<DataSet> ExcuteStoreAsync(string StoreName, dynamic Params)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(StoreName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlParameterBuilder.AddParameters(command, Params);

            await conn.OpenAsync();
            DataSet ds = new DataSet();
            var adapter = new SqlDataAdapter(command);

            adapter.Fill(ds);
            return ds;
        }
    }
}
