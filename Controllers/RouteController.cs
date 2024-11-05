using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Program.Models;
using System.Collections.Generic;
using System.IO;

namespace Program.Controllers
{
    public class RouteController : Controller
    {
        private readonly IConfiguration _configuration;

        public RouteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<RouteModel> routeList = new List<RouteModel>();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM routes", connection);
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

        // GET: Route/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Route/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RouteModel model)
        {
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("INSERT INTO routes (direction, distance) VALUES (@direction, @distance)", connection);
                    command.Parameters.AddWithValue("direction", model.direction);
                    command.Parameters.AddWithValue("distance", model.distance);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM routes WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
