using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.ViewModels.Home;

namespace XWingTO.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Tournament, Guid> _tournamentRepository;

	    public HomeController(UserManager<ApplicationUser> userManager, IRepository<Tournament, Guid> tournamentRepository)
	    {
            _userManager = userManager;
	    }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
	            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                Guid userId = user.Id;

                var myEvents = _tournamentRepository.Query().Where(t => t.TOId == userId || t.Players.Select(p => p.PlayerId).Contains(userId));

                MyHomeViewModel model = new MyHomeViewModel
                {
	                UpcomingEvents = await myEvents.Where(t => t.Date >= DateOnly.FromDateTime(DateTime.Today)).ExecuteAsync(),
	                PreviousEvents = await myEvents.Where(t => t.Date < DateOnly.FromDateTime(DateTime.Today)).ExecuteAsync(),
                };

				return View("MyHome", model);
            }
            else
            {
	            List<Tournament> recentEvents = await _tournamentRepository.Query()
		            .Order(t => t.OrderByDescending<Tournament, DateTime>(t => t.CreationDate)).Take(10).ExecuteAsync();

                return View();
            }
        }
    }
}
