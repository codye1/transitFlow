using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TransitFlow.mvc.Models;
using TransitFlow.mvc.Models.DTO;

namespace TransitFlow.mvc.Controllers
{
    public class RouteController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RouteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] int? afterId, [FromQuery] int take = 10)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.GetAsync($"/routes?afterId={afterId}&take={take}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResponseDto<RouteModel>>();
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("/routes")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto routeData)
        {
            var client = _httpClientFactory.CreateClient("TransitApi");
            var response = await client.PostAsJsonAsync("/routes", routeData);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized();
            if (!response.IsSuccessStatusCode)
                return BadRequest();

            var createdRoute = await response.Content.ReadFromJsonAsync<RouteModel>();
            if (createdRoute == null)
                return BadRequest();

            var itemModel = new RouteItemViewModel
            {
                Route = createdRoute,
                User = GetUser()
            };

            return PartialView("~/Views/Home/Components/RouteSidebar/Partials/_RouteItem.cshtml", itemModel);
        }

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