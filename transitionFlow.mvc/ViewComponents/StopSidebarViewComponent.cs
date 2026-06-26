using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransitFlow.mvc.Models;

namespace TransitFlow.mvc.ViewComponents
{
    public class StopSidebarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<StopModel> stops,HomeUserModel user)
        {
            var model = new StopSidebarViewModel
            {
                Stops = stops,
                User = user
            };

            return View(model);
        }
    }
}