using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Program.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Program.Controllers
{
    public class FlightController : Controller
    {
        private readonly string _connectionString;

        public FlightController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            ViewBag.Pilots = GetPilots();
            ViewBag.Routes = GetRoutes();
            ViewBag.Planes = GetPlanes();
            List<FlightModel> flightList = new List<FlightModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM flights", connection);
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

        // GET: Flight/Create
        public IActionResult Create()
        {
            // Получаем данные для выпадающих списков
            ViewBag.Pilots = GetPilots();
            ViewBag.Routes = GetRoutes();
            ViewBag.Planes = GetPlanes();
            return View();
        }

        // POST: Flight/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FlightModel flight)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("INSERT INTO flights (start_time, route_id, pilot_id, plane_id, passenger_count) VALUES (@start_time, @route_id, @pilot_id, @plane_id, @passenger_count)", connection);
                    command.Parameters.AddWithValue("@start_time", flight.start_time);
                    command.Parameters.AddWithValue("@route_id", flight.route_id);
                    command.Parameters.AddWithValue("@pilot_id", flight.pilot_id);
                    command.Parameters.AddWithValue("@plane_id", flight.plane_id);
                    command.Parameters.AddWithValue("@passenger_count", flight.passenger_count);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction(nameof(Index));
            }

            // Если модель не валидна, возвращаем данные для выпадающих списков
            ViewBag.Pilots = GetPilots();
            ViewBag.Routes = GetRoutes();
            ViewBag.Planes = GetPlanes();
            return View(flight);
        }

        private List<PilotModel> GetPilots()
        {
            List<PilotModel> pilots = new List<PilotModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT id, name, surname FROM pilots", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pilots.Add(new PilotModel
                    {
                        id = reader.GetGuid(0),
                        name = reader.GetString(1),
                        surname = reader.GetString(2),
                        full_name = $"{reader.GetString(1)} {reader.GetString(2)}" // Объединение имени и фамилии
                    });
                }
                connection.Close();
            }
            return pilots;
        }

        private List<RouteModel> GetRoutes()
        {
            List<RouteModel> routes = new List<RouteModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM routes", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    routes.Add(new RouteModel
                    {
                        id = reader.GetInt32(0),
                        direction = reader.GetString(1)
                    });
                }
                connection.Close();
            }
            return routes;
        }

        private List<PlaneModel> GetPlanes()
        {
            List<PlaneModel> planes = new List<PlaneModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM planes", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    planes.Add(new PlaneModel
                    {
                        id = reader.GetInt32(0),
                        mark = reader.GetString(1)  
                    });
                }
                connection.Close();
            }
            return planes;
        }

        public IActionResult Delete(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM flights WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}