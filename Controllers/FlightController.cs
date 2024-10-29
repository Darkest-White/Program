using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;

namespace Program.Controllers
{
    public class FlightController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<FlightModel> flightList = new List<FlightModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from flights", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var flight = new FlightModel
                    {
                        id = reader.GetInt32(0),
                        start_time = reader.GetDateTime(1),
                        route_id = reader.GetInt32(2),
                        pilot_id = reader.GetGuid(3),
                        plane_id = reader.GetInt32(4),
                        passenger_count = reader.GetInt32(5)
                    };
                    flightList.Add(flight);
                }
                connection.Close();
            }
            return View(flightList);
        }
    }
}