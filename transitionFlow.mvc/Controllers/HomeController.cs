using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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

        var stopsTask = _httpClient.GetAsync("stops?take=100");
        var routesTask = _httpClient.GetAsync("routes?take=100"); 

        await Task.WhenAll(stopsTask, routesTask);

        var stopsResponse = await stopsTask;
        var routesResponse = await routesTask;

        List<StopModel> stops = new();
        if (stopsResponse.IsSuccessStatusCode)
        {
            var json = await stopsResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StopsPagedResponse>(json, jsonOptions);
            stops = result?.Data ?? new();
        }

        List<RouteModel> routes = new();
        if (routesResponse.IsSuccessStatusCode)
        {
            var json = await routesResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RoutesPagedResponse>(json, jsonOptions);
            routes = result?.Data ?? new();
        }

        var model = new HomeViewModel
        {
            Stops = stops,
            Routes = routes,
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

public class RoutesPagedResponse
{
    public List<RouteModel> Data { get; set; } = new();
    public bool HasMore { get; set; }
}