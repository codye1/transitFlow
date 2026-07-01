using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TransitFlow.mvc.Models;
using TransitFlow.mvc.Models.DTO;

namespace TransitFlow.mvc.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VehiclesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /vehicles
        [HttpGet("/vehicles")]
        public async Task<IActionResult> GetVehicles([FromQuery] int? afterId, [FromQuery] int take = 10)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");

            var response = await client.GetAsync($"/vehicles?afterId={afterId}&take={take}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResponseDto<VehicleModel>>();
                return Ok(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            return BadRequest();
        }

        // POST: /vehicles
        [HttpPost("/vehicles")]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto vehicleData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/vehicles", vehicleData);

            if (response.IsSuccessStatusCode)
            {
                var createdVehicle = await response.Content.ReadFromJsonAsync<VehicleModel>();
                if (createdVehicle == null)
                    return BadRequest();

                var routesResponse = await client.GetAsync("/routes?take=100");
                var routes = new List<RouteModel>();

                if (routesResponse.IsSuccessStatusCode)
                {
                    var json = await routesResponse.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var result = JsonSerializer.Deserialize<PagedResponseDto<RouteModel>>(json, jsonOptions);
                    routes = result?.Data ?? new();
                }

                var itemModel = new VehicleItemViewModel
                {
                    Vehicle = createdVehicle,
                    Routes = routes,
                    User = GetUser()
                };

                return PartialView("~/Views/Home/Components/VehicleSidebar/Partials/_VehicleItem.cshtml", itemModel);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<object>();
                return BadRequest(errorResult);
            }

            return BadRequest();
        }

        // DELETE: /vehicles/{id}
        [HttpDelete("/vehicles/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.DeleteAsync($"/vehicles/{id}");

            if (response.IsSuccessStatusCode)
                return NoContent();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return StatusCode(403);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            return BadRequest();
        }

        // PATCH: /vehicles/{id}
        [HttpPatch("/vehicles/{id}")]
        public async Task<IActionResult> PatchVehicle(int id, [FromBody] PatchVehicleDto vehicleData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PatchAsJsonAsync($"/vehicles/{id}", vehicleData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return StatusCode(403);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<object>();
                return BadRequest(errorResult);
            }

            return BadRequest();
        }
    }
}