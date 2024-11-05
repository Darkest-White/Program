using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Program.Models;
using System.Collections.Generic;
using System.IO;

namespace Program.Controllers
{
    public class PlaneController : Controller
    {
        private readonly IConfiguration _configuration;

        public PlaneController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<PlaneModel> planeList = new List<PlaneModel>();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM planes", connection);
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

        // GET: Plane/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plane/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PlaneModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("INSERT INTO planes (mark, number_of_seats) VALUES (@mark, @number_of_seats)", connection);
                    command.Parameters.AddWithValue("mark", model.mark);
                    command.Parameters.AddWithValue("number_of_seats", model.number_of_seats);
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
                var command = new NpgsqlCommand("DELETE FROM planes WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
