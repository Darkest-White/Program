using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;

namespace Program.Controllers
{
    public class PilotController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<PilotModel> pilotList = new List<PilotModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from pilots", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var pilot = new PilotModel
                    {
                        id = reader.GetGuid(0),
                        name = reader.GetString(1),
                        surname = reader.GetString(2),
                        full_name = reader.GetString(3)
                    };
                    pilotList.Add(pilot);
                }
                connection.Close();
            }
            return View(pilotList);
        }
    }
}
