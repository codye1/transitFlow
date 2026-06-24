using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TransitFlow.mvc.Models;

namespace TransitFlow.mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new TransitViewModel
            {
                Routes = new List<RouteModel>
                {
                    new() { Id = "r1", Number = "12А", Name = "Вокзал — Аеропорт", Color = "#3b82f6", Status = "active", Stops = new[] { "s1", "s2", "s3" } },
                    new() { Id = "r2", Number = "47", Name = "Центр — Східний", Color = "#f59e0b", Status = "active", Stops = new[] { "s2", "s3", "s4" } }
                },
                Stops = new List<StopModel>
                {
                    new() { Id = "s1", Name = "Залізничний вокзал", Latitude = 48.1444, Longitude = 23.0325, Type = "bus" },
                    new() { Id = "s2", Name = "Площа Миру", Latitude = 48.1462, Longitude = 23.0378, Type = "combined" },
                    new() { Id = "s3", Name = "Університет", Latitude = 48.1411, Longitude = 23.0410, Type = "bus" },
                    new() { Id = "s4", Name = "Аеропорт", Latitude = 48.1385, Longitude = 23.0522, Type = "bus" }
                },
                Vehicles = new List<VehicleModel>
                {
                    new() { Id = "v1", Status = "on_route", RouteId = "r1" },
                    new() { Id = "v2", Status = "on_route", RouteId = "r1" },
                    new() { Id = "v3", Status = "maintenance", RouteId = null }
                }
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            ViewBag.RoutesJson = JsonSerializer.Serialize(model.Routes, jsonOptions);
            ViewBag.StopsJson = JsonSerializer.Serialize(model.Stops, jsonOptions);
            ViewBag.VehiclesJson = JsonSerializer.Serialize(model.Vehicles, jsonOptions);

            return View(model);
        }
    }
}