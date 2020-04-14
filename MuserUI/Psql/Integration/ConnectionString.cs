using Microsoft.Extensions.Configuration;
using Tolltech.PostgreEF.Integration;

namespace Tolltech.MuserUI.Psql.Integration
{
    public class ConnectionString : IConnectionString
    {
        public ConnectionString(IConfiguration configuration)
        {
            Value = configuration["connectionString"];
        }

        public string Value { get; }
    }
}