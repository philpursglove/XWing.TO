using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.ViewModels;
using XWingTO.Web.ViewModels.Home;

namespace XWingTO.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Tournament, Guid> _tournamentRepository;
        private readonly IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;

	    public HomeController(UserManager<ApplicationUser> userManager, IRepository<Tournament, Guid> tournamentRepository, 
		    IRepository<TournamentPlayer, Guid> tournamentPlayerRepository)
	    {
            _userManager = userManager;
            _tournamentRepository = tournamentRepository;
            _tournamentPlayerRepository = tournamentPlayerRepository;
	    }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
	            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                Guid userId = user.Id;

                var myTOEvents = await _tournamentRepository.Query().Where(t => t.TOId == userId).ExecuteAsync();
                var myTournamentPlayers = _tournamentPlayerRepository.Query().Where(t => t.PlayerId == userId);
                var myPlayEvents = await myTournamentPlayers.Select(t => t.Tournament).ExecuteAsync();
                
                List<TournamentListDisplayModel> upcomingEvents = new List<TournamentListDisplayModel>();
                List<TournamentListDisplayModel> previousEvents = new List<TournamentListDisplayModel>();

                var myEvents = myTOEvents.Union(myPlayEvents);

                foreach (Tournament tournament in myEvents.Where(t => t.Date >= DateOnly.FromDateTime(DateTime.Today)).Take(10))
                {
	                upcomingEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
		                tournament.Players));
                }

                foreach (Tournament tournament in myEvents.Where(t => t.Date < DateOnly.FromDateTime(DateTime.Today)).Take(10))
                {
	                previousEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
		                tournament.Players));
                }

                MyHomeViewModel model = new MyHomeViewModel
                {
	                UpcomingEvents = upcomingEvents,
	                PreviousEvents = previousEvents,
                };

				return View("MyHome", model);
            }
            else
            {
	            List<Tournament> recentEvents = await _tournamentRepository.Query()
		            .Order(t => t.OrderByDescending<Tournament, DateTime>(t => t.CreationDate)).Take(10).ExecuteAsync();
	            List<TournamentListDisplayModel> tournaments = new List<TournamentListDisplayModel>(10);

	            foreach (Tournament recentEvent in recentEvents)
	            {
		            tournaments.Add(new TournamentListDisplayModel(recentEvent.Id, recentEvent.Name, recentEvent.Date, recentEvent.Players));
	            }
                return View("Index", tournaments);
            }
        }
    }
}
