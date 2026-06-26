using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransitFlow.mvc.Models;

namespace TransitFlow.mvc.ViewComponents
{
    public class RouteSidebarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<RouteModel> routes, IEnumerable<StopModel> stops, HomeUserModel user)
        {
            var viewModel = new RouteSidebarViewModel
            {
                Routes = routes,
                AllStops = stops,
                User = user
            };

            return View(viewModel);
        }
    }
}