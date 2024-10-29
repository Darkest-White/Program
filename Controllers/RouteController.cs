using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;

namespace Program.Controllers
{
    public class RouteController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<RouteModel> routeList = new List<RouteModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
                var command = new NpgsqlCommand("select * from routes", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var route = new RouteModel
                    {
                        id = reader.GetInt32(0),
                        direction = reader.GetString(1),
                        distance = reader.GetInt32(2)
                    };
                    routeList.Add(route);
                }
                connection.Close();
            }
            return View(routeList);
        }
    }
}
