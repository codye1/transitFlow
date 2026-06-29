using Microsoft.EntityFrameworkCore;
using System;
using transitFlow.api.Data;
using transitFlow.api.Models;
using transitFlow.api.Repositories.Vehicle;

namespace transitFlow.api.Repositories.Vehicle
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly TransitFlowDbContext _context;

        public VehicleRepository(TransitFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Vehicle>> GetVehiclesCursorAsync(int? afterId, int take)
        {
            var query = _context.Vehicles.AsNoTracking();

            if (afterId.HasValue && afterId.Value > 0)
            {
                query = query.Where(v => v.Id > afterId.Value);
            }

            return await query
                .OrderBy(v => v.Id) 
                .Take(take+1)
                .ToListAsync();
        }

        public async Task<Models.Vehicle> CreateVehicleAsync(Models.Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return false;
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleAsync(Models.Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return true; 
        }
        public async Task<Models.Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }
        public async Task<(bool PlateExists, bool RouteExists)> ValidateVehicleCreationAsync(string plateNumber, int? routeId)
        {
            var result = await _context.Users
                .Take(1)
                .Select(_ => new
                {
                    PlateExists = _context.Vehicles.Any(v => v.PlateNumber == plateNumber),
                    RouteExists = routeId.HasValue
                        ? _context.Routes.Any(r => r.Id == routeId.Value)
                        : true
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                bool plateExists = await _context.Vehicles.AnyAsync(v => v.PlateNumber == plateNumber);
                bool routeExists = !routeId.HasValue || await _context.Routes.AnyAsync(r => r.Id == routeId.Value);
                return (plateExists, routeExists);
            }

            return (result.PlateExists, result.RouteExists);
        }
    }
}