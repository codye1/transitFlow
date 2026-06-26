using Microsoft.AspNetCore.Mvc;

namespace TransitFlow.mvc.Controllers
{
    public class StopsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StopsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("/stops")]
        public async Task<IActionResult> CreateStop([FromBody] object stopData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/stops", stopData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized();

            return BadRequest();
        }

        [HttpDelete("/stops/{id}")]
        public async Task<IActionResult> DeleteStop(int id)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.DeleteAsync($"/stops/{id}");

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

        [HttpPut("/stops/{id}")]
        public async Task<IActionResult> UpdateStop(int id, [FromBody] object stopData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");

            // Відправляємо PUT запит на Web API
            var response = await client.PutAsJsonAsync($"/stops/{id}", stopData);

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