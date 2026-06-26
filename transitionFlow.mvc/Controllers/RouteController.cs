using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TransitFlow.mvc.Controllers
{
    public class RouteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RouteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /routes Proxy (Handles cursor pagination parameters)
        [HttpGet("/routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] int? afterId, [FromQuery] int take = 10)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");

            // Forward the query string parameters directly to the API
            var response = await client.GetAsync($"/routes?afterId={afterId}&take={take}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }

            return BadRequest();
        }

        // POST: /routes Proxy
        [HttpPost("/routes")]
        public async Task<IActionResult> CreateRoute([FromBody] object routeData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/routes", routeData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized();

            return BadRequest();
        }

        // DELETE: /routes/{id} Proxy
        [HttpDelete("/routes/{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.DeleteAsync($"/routes/{id}");

            if (response.IsSuccessStatusCode)
                return NoContent();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return StatusCode(403);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            return BadRequest();
        }
    }
}