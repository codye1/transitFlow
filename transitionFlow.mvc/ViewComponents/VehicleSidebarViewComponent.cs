using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransitFlow.mvc.Models;

namespace TransitFlow.mvc.ViewComponents
{
    public class VehicleSidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<VehicleModel> vehicles, List<RouteModel> routes, HomeUserModel? user)
        {
            var viewModel = new VehicleSidebarViewModel
            {
                Vehicles = vehicles,
                Routes = routes,
                User = user,
            };
                
            return View(viewModel);
        }
    }
}