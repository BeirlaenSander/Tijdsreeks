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
using tijdsreeks_groep2.Models;

namespace MCT.Functions
{
    public class DagenFunctions
    {
        [FunctionName("GetDagen")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req,
            ILogger log)
        {
            try
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
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetVisitors")]
        public async Task<IActionResult> GetVisitors([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "visitors/{day}")] HttpRequest req,
            string day,
            ILogger log)
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
                    List<Visits> visits = new List<Visits>();
                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using(SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = "SELECT TijdstipDag, AantalBezoekers FROM Bezoekers WHERE DagVanDeWeek = @dag";
                            command.Parameters.AddWithValue("@dag",day);

                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            while (await reader.ReadAsync())
                            {
                                var visit = new Visits();
                                visit.Tijdstip=Convert.ToInt32(reader["TijdstipDag"]);
                                visit.AantalBezoekers = Convert.ToInt32(reader["AantalBezoekers"]);
                                visit.DagVanDeWeek = day;
                                visits.Add(visit);
                            }
                        }
                    }
                    return new OkObjectResult(visits);
                }
                catch (System.Exception ex)
                {
                    log.LogError(ex.Message);
                    return new StatusCodeResult(500);
                }
            }
    }
}



