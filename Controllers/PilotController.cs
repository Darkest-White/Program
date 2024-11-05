using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;
using Microsoft.Extensions.Configuration;

namespace Program.Controllers
{
    public class PilotController : Controller
    {
        private readonly string _connectionString;

        public PilotController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<PilotModel> pilotList = new List<PilotModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM pilots", connection);
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

        // GET: Pilot/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pilot/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PilotModel pilot)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("INSERT INTO pilots (id, name, surname) VALUES (@id, @name, @surname)", connection);
                    command.Parameters.AddWithValue("@id", Guid.NewGuid()); // Генерация нового UUID
                    command.Parameters.AddWithValue("@name", pilot.name);
                    command.Parameters.AddWithValue("@surname", pilot.surname);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pilot);
        }
        public IActionResult Delete(Guid id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM pilots WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
