using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TransitFlow.mvc.Models;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TransitApi");
    }

    public async Task<IActionResult> Index()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var response = await _httpClient.GetAsync("stops?take=100");

        List<StopModel> stops = new();
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StopsPagedResponse>(json, jsonOptions);
            stops = result?.Data ?? new();
        }

        var model = new HomeViewModel
        {
            Stops = stops,
            Routes = new List<RouteModel>(),
            Vehicles = new List<VehicleModel>()
        };

        return View(model);
    }
}

public class StopsPagedResponse
{
    public List<StopModel> Data { get; set; } = new();
    public bool HasMore { get; set; }
}