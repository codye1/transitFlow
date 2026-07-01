using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TransitFlow.mvc.Models;
using System.IdentityModel.Tokens.Jwt;
using TransitFlow.mvc.Models.DTO;

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
        var vehiclesTask = _httpClient.GetAsync("vehicles?take=100");

        await Task.WhenAll(stopsTask, routesTask, vehiclesTask);

        var stopsResponse = await stopsTask;
        var routesResponse = await routesTask;
        var vehiclesResponse = await vehiclesTask;

        List<StopModel> stops = new();
        if (stopsResponse.IsSuccessStatusCode)
        {
            var json = await stopsResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResponseDto<StopModel>>(json, jsonOptions);
            stops = result?.Data ?? new();
        }

        List<RouteModel> routes = new();
        if (routesResponse.IsSuccessStatusCode)
        {
            var json = await routesResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResponseDto<RouteModel>>(json, jsonOptions);
            routes = result?.Data ?? new();
        }

        List<VehicleModel> vehicles = new();
        if (vehiclesResponse.IsSuccessStatusCode)
        {
            var json = await vehiclesResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResponseDto<VehicleModel>>(json, jsonOptions);
            vehicles = result?.Data ?? new();
        }

        var model = new HomeViewModel
        {
            Stops = stops,
            Routes = routes,
            Vehicles = vehicles,
            User = GetUser()
        };

        return View(model);
    }

    private HomeUserModel? GetUser()
    {
        var accessToken = Request.Cookies["accessToken"];
        if (string.IsNullOrEmpty(accessToken)) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(accessToken)) return null;

            var jwtToken = handler.ReadJwtToken(accessToken);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int parsedId))
            {
                return null;
            }

            var roleClaims = jwtToken.Claims
                .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            var userRoles = new List<AppRole>();
            foreach (var roleStr in roleClaims)
            {
                if (Enum.TryParse<AppRole>(roleStr, true, out var parsedRole))
                {
                    userRoles.Add(parsedRole);
                }
            }

            return new HomeUserModel
            {
                Id = parsedId,
                Roles = userRoles
            };
        }
        catch
        {
            return null;
        }
    }
}