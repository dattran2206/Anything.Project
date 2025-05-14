using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.DataAccess.Helpers
{
    public static class SqlParameterBuilder
    {
        public static void AddParameters(SqlCommand command, dynamic param)
        {
            if (param == null) return;
            var properties = param.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(param);
                if (value == null)
                {
                    command.Parameters.AddWithValue($"@{property.Name}", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue($"@{property.Name}", value);
                }
            }
        }
    }
}
