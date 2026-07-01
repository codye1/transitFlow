using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TransitFlow.mvc.Controllers
{
    public class VehiclesController : Controller
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
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            return BadRequest();
        }

        // POST: /vehicles
        [HttpPost("/vehicles")]
        public async Task<IActionResult> CreateVehicle([FromBody] object vehicleData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/vehicles", vehicleData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result); // Or Created() depending on how your UI expects it
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
        public async Task<IActionResult> PatchVehicle(int id, [FromBody] object vehicleData)
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