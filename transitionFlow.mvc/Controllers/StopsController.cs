using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TransitFlow.mvc.Models;
using TransitFlow.mvc.Models.DTO;

namespace TransitFlow.mvc.Controllers
{
    public class StopsController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StopsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("/stops")]
        public async Task<IActionResult> CreateStop([FromBody] StopCreateDto stopData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/stops", stopData);

            if (response.IsSuccessStatusCode)
            {
                var createdStop = await response.Content.ReadFromJsonAsync<StopModel>();
                if (createdStop == null)
                    return BadRequest();

                var itemModel = new StopItemViewModel
                {
                    Stop = createdStop,
                    User = GetUser()
                };

                return PartialView("~/Views/Home/Components/StopSidebar/Partials/_StopItem.cshtml", itemModel);
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
        public async Task<IActionResult> UpdateStop(int id, [FromBody] StopUpdateDto stopData)
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