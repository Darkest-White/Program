using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;

namespace Program.Controllers
{
    public class PlaneController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<PlaneModel> planeList = new List<PlaneModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from planes", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var plane = new PlaneModel
                    {
                        id = reader.GetInt32(0),
                        mark = reader.GetString(1),
                        number_of_seats = reader.GetInt32(2)
                    };
                    planeList.Add(plane);
                }
                connection.Close();
            }
            return View(planeList);
        }
    }
}
