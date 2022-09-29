using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace MCT.Functions
{
    public class GetDagen
    {
        [FunctionName("GetDagen")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req,
            ILogger log)
        {
            List<string> days = new List<string>();
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using(SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = "SELECT DISTINCT DagVanDeWeek FROM Bezoekers";
                    command.CommandText = sql;

                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        days.Add(reader["DagVanDeWeek"].ToString());
                    }
                }
            }
            return new OkObjectResult(days);
        }
    }
}
