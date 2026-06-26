using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using transitFlow.api.Models;
using transitFlow.api.Models.DTO.Stop;
using transitFlow.api.Repositories;

namespace transitFlow.api.Controllers
{
    [Route("stops")]
    [ApiController]
    public class StopsController : ControllerBase
    {
        private readonly IStopRepository _stopRepository;
                                                                                                             
        public StopsController(IStopRepository stopRepository)
        {
            _stopRepository = stopRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetStops([FromQuery] int? afterId, [FromQuery] int take = 10, [FromQuery] int? routeId = null)
        {
            if (take < 1 || take > 100) take = 10;

            var stops = await _stopRepository.GetStopsCursorAsync(afterId, take, routeId);
            var stopList = stops.ToList();

            bool hasMore = stopList.Count > take;

            if (hasMore)
            {
                stopList.RemoveAt(take);
            }

            var itemsResponse = stopList.Select(s => new StopResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                CreatedAt = s.CreatedAt
            }).ToList();

            return Ok(new
            {
                Data = itemsResponse,
                HasMore = hasMore
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<StopResponseDto>> CreateStop([FromBody] StopCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var stop = new Stop
            {                                                                                                                                                                       
                Name = dto.Name,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _stopRepository.CreateAsync(stop);

            var response = new StopResponseDto
            {
                Id = stop.Id,
                Name = stop.Name,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                CreatedAt = stop.CreatedAt,
            };

            return CreatedAtAction(nameof(GetStops), new { id = stop.Id }, response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStop(int id)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null)
                return NotFound($"Stop with ID {id} not found.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("User ID not found in token.");

            bool isAdminOrModerator = User.IsInRole("admin") || User.IsInRole("moderator");

            if (!isAdminOrModerator && stop.CreatedById != userId)
                return Forbid();

            await _stopRepository.DeleteAsync(stop);

            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStop(int id, [FromBody] StopUpdateDto dto)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null)
                return NotFound($"Stop with ID {id} not found.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("User ID not found in token.");

            bool isAdminOrModerator = User.IsInRole("admin") || User.IsInRole("moderator");
            if (!isAdminOrModerator && stop.CreatedById != userId)
                return Forbid();

            stop.Name = dto.Name;
            stop.Latitude = dto.Latitude;
            stop.Longitude = dto.Longitude;

            await _stopRepository.UpdateAsync(stop);

            return NoContent();
        }
    }
}