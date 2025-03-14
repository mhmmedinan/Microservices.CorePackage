using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core.CrossCuttingConcerns.Utilities.Helpers.IPControl;

public class IPControlHelper : IIPControlHelper
{
    private readonly Configuration _configuration;

    public IPControlHelper(IConfiguration configuration)
    {
        const string configurationSection = "ConnectionStrings:IPControlConnectionString";
        _configuration = configuration.GetSection(configurationSection).Get<Configuration>() 
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration."); ;
    }
    public List<string> GetAllowedIPListAsync()
    {
        var allowedIPs = new List<string>();

        using (var connection = new SqlConnection(_configuration.ConnectionString))
        {
             connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT IpAddress FROM WhiteLists WHERE IpAddress IS NOT NULL AND IpAddress != ''";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allowedIPs.Add(reader.GetString(0));
                    }
                }
            }
        }
        return allowedIPs.ToList();

    }
}
