using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using transitFlow.api.Models;
using transitFlow.api.Models.DTO.Route;
using transitFlow.api.Repositories;
using transitFlow.api.Repositories.Route;
using RouteEntity = transitFlow.api.Models.Route;

namespace transitFlow.api.Controllers
{
    [ApiController]
    [Route("routes")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IStopRepository _stopRepository;

        public RoutesController(IRouteRepository routeRepository, IStopRepository stopRepository)
        {
            _routeRepository = routeRepository;
            _stopRepository = stopRepository;
        }

        // GET: /routes
        [HttpGet]
        public async Task<ActionResult> GetRoutes([FromQuery] int? afterId, [FromQuery] int take = 10)
        {
            if (take < 1 || take > 100) take = 10;

            var routes = await _routeRepository.GetRoutesCursorAsync(afterId, take);
            var routeList = routes.ToList();

            bool hasMore = routeList.Count > take;

            if (hasMore)
            {
                routeList.RemoveAt(take);
            }

            var response = routeList.Select(r => new RouteResponseDto
            {
                Id = r.Id,
                Number = r.Number,
                Name = r.Name,
                Color = r.Color,
                Stops = r.RouteStops
                    .OrderBy(rs => rs.SequenceNumber)
                    .Select(rs => rs.StopId)
                    .ToList(),
                CreatedById= r.CreatedById
            }).ToList();

            return Ok(new
            {
                Data = response,
                HasMore = hasMore
            });
        }

        // POST: /routes
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RouteResponseDto>> CreateRoute([FromBody] CreateRouteDto dto)
        {
            if (dto == null) return BadRequest();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return Unauthorized();
            

            if (!await _stopRepository.AreStopsValidAsync(dto.SelectedStops))
                return BadRequest("One or more selected stops do not exist.");

            var newRoute = new RouteEntity
            {
                Number = dto.Number,
                Name = dto.Name,
                Color = dto.Color,
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId
            };

            if (dto.SelectedStops != null && dto.SelectedStops.Any())
            {
                for (int i = 0; i < dto.SelectedStops.Count; i++)
                {
                    newRoute.RouteStops.Add(new RouteStop
                    {
                        StopId = dto.SelectedStops[i],
                        SequenceNumber = i + 1 
                    });
                }
            }

            await _routeRepository.CreateRouteAsync(newRoute);

            var response = new RouteResponseDto
            {
                Id = newRoute.Id,
                Number = dto.Number,
                Name = newRoute.Name,
                Type = dto.Type,
                Color = dto.Color,
                Stops = newRoute.RouteStops
                    .OrderBy(rs => rs.SequenceNumber)
                    .Select(rs => rs.StopId)
                    .ToList()
            };

            return CreatedAtAction(nameof(GetRoutes), new { id = newRoute.Id }, response);
        }

        // DELETE: /routes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var deleted = await _routeRepository.DeleteRouteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}