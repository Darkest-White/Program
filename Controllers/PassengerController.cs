using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Program.Models;
using System.Collections.Generic;
using System.IO;

namespace Program.Controllers
{
    public class PassengerController : Controller
    {
        private readonly IConfiguration _configuration;

        public PassengerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.Flights = GetFlights();
            List<PassengerModel> passengerList = new List<PassengerModel>();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM passengers", connection);
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

        // Метод для получения доступных flight_id из таблицы flights
        private List<int> GetAvailableFlightIds()
        {
            List<int> flightIds = new List<int>();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT id FROM flights", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    flightIds.Add(reader.GetInt32(0));
                }
                connection.Close();
            }
            return flightIds;
        }

        // GET: Passenger/Create
        public IActionResult Create()
        {
            ViewBag.FlightIds = GetAvailableFlightIds(); // Передаём список flight_id в представление через ViewBag
            return View();
        }

        // POST: Passenger/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PassengerModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("INSERT INTO passengers (name, surname, flight_id, full_name) VALUES (@name, @surname, @flight_id, @full_name)", connection);
                    command.Parameters.AddWithValue("name", model.name);
                    command.Parameters.AddWithValue("surname", model.surname);
                    command.Parameters.AddWithValue("flight_id", model.flight_id);
                    command.Parameters.AddWithValue("full_name", $"{model.name} {model.surname}");
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction("Index");
            }

            // Если модель невалидна, возвращаем список flight_id обратно в представление
            ViewBag.FlightIds = GetAvailableFlightIds();
            return View(model);
        }

        private List<FlightModel> GetFlights()
        {
            List<FlightModel> flights = new List<FlightModel>();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM flights", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    flights.Add(new FlightModel
                    {
                        id = reader.GetInt32(0),
                        start_time = reader.GetDateTime(1),
                        route_id = reader.GetInt32(2),
                        pilot_id = reader.GetGuid(3),
                        plane_id = reader.GetInt32(4),
                        passenger_count = reader.GetInt32(5)
                    });
                }
                connection.Close();
            }
            return flights;
        }

        // POST: Flight/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM passengers WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM passengers WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
