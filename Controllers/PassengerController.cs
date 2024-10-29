using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;

namespace Program.Controllers
{
    public class PassengerController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<PassengerModel> passengerList = new List<PassengerModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from passengers", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var passenger = new PassengerModel
                    {
                        id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        surname = reader.GetString(2),
                        flight_id = reader.GetInt32(3),
                        full_name = reader.GetString(4)
                    };
                    passengerList.Add(passenger);
                }
                connection.Close();
            }
            return View(passengerList);
        }
    }
}
