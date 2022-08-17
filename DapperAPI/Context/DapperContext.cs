using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperAPI.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _ConnectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _ConnectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_ConnectionString);

    }
}
