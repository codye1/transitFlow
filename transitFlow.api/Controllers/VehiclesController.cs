using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using transitFlow.api.Models;
using transitFlow.api.Models.DTO;
using transitFlow.api.Models.DTO.Vehicle;
using transitFlow.api.Repositories.Vehicle;

namespace transitFlow.api.Controllers
{
    [ApiController]
    [Route("vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleRepository vehicleRepository, ILogger<VehiclesController> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetVehicles([FromQuery] int? afterId, [FromQuery] int take = 10)
        {
            if (take is < 1 or > 100) take = 10;

            var vehicles = await _vehicleRepository.GetVehiclesCursorAsync(afterId, take);
            var vehicleList = vehicles.ToList();

            bool hasMore = vehicleList.Count > take;

            if (hasMore)
            {
                vehicleList.RemoveAt(take);
            }

            var itemsResponse = vehicleList.Select(v => new VehicleResponseDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Type = v.Type,
                Model = v.Model,
                RouteId = v.RouteId,
                Capacity = v.Capacity,
                CreatedAt = v.CreatedAt,
                CreatedById = v.CreatedById,
                Status = v.Status
            }).ToList();

            return Ok(new
            {
                Data = itemsResponse,
                HasMore = hasMore
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<VehicleResponseDto>> CreateVehicle([FromBody] CreateVehicleDto dto)
        {
            if (dto is null) return BadRequest(ApiErrors.General("Request body is required."));

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(ApiErrors.General("User ID not found in token."));
            }

            _logger.LogInformation("Started validation");
            var (plateExists, routeExists) = await _vehicleRepository.ValidateVehicleCreationAsync(dto.PlateNumber, dto.RouteId);

            var errors = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);

            if (plateExists)
            {
                errors[nameof(dto.PlateNumber)] = ["A vehicle with this plate number already exists."];
            }

            if (!routeExists)
            {
                errors[nameof(dto.RouteId)] = ["The specified Route ID does not exist."];
            }

            if (errors.Count > 0)
            {
                return BadRequest(ApiErrors.FromDictionary(errors));
            }

            _logger.LogInformation("Finished VALIDATION");

            var newVehicle = new Models.Vehicle
            {
                PlateNumber = dto.PlateNumber,
                Type = dto.Type,
                Model = dto.Model,
                RouteId = dto.RouteId,
                Capacity = dto.Capacity,
                CreatedAt = DateTime.UtcNow,
                Status = dto.Status,
                CreatedById = userId
            };

            _logger.LogInformation("Creating vehicle");
            await _vehicleRepository.CreateVehicleAsync(newVehicle);

            var response = new VehicleResponseDto
            {
                Id = newVehicle.Id,
                PlateNumber = newVehicle.PlateNumber,
                Type = newVehicle.Type,
                Model = newVehicle.Model,
                Status = newVehicle.Status,
                RouteId = newVehicle.RouteId,
                Capacity = newVehicle.Capacity,
                CreatedAt = newVehicle.CreatedAt,
                CreatedById = newVehicle.CreatedById
            };

            _logger.LogInformation("Created, returning");
            return CreatedAtAction(nameof(GetVehicles), new { id = newVehicle.Id }, response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var deleted = await _vehicleRepository.DeleteVehicleAsync(id);
            if (!deleted) return NotFound(ApiErrors.General($"Vehicle with ID {id} not found."));

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult<VehicleResponseDto>> PatchVehicle(int id, [FromBody] PatchVehicleDto dto)
        {
            if (dto is null) return BadRequest(ApiErrors.General("Request body is required."));

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null) return NotFound(ApiErrors.General($"Vehicle with ID {id} not found."));

            bool hasChanges = false;

            if (dto.RouteId.HasValue)
            {
                vehicle.RouteId = dto.RouteId.Value;
                hasChanges = true;
            }

            if (dto.Status.HasValue)
            {
                vehicle.Status = dto.Status.Value;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                _logger.LogWarning("No trackable changes detected in payload.");
                return Ok(MapToResponseDto(vehicle));
            }

            await _vehicleRepository.UpdateVehicleAsync(vehicle);

            _logger.LogInformation("Vehicle {Id} updated successfully.", id);
            return Ok(MapToResponseDto(vehicle));
        }

        private VehicleResponseDto MapToResponseDto(Models.Vehicle v)
        {
            return new VehicleResponseDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Type = v.Type,
                Model = v.Model,
                RouteId = v.RouteId,
                Capacity = v.Capacity,
                Status = v.Status,
                CreatedAt = v.CreatedAt,
                CreatedById = v.CreatedById
            };
        }
    }
}