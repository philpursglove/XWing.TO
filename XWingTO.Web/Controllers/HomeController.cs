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
	        ApplicationUser TO;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
	            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                Guid userId = user.Id;

                var myTOEvents = await _tournamentRepository.Query().Include(t => t.Players).Where(t => t.TOId == userId).ExecuteAsync();
                var myTournamentPlayers = _tournamentPlayerRepository.Query().Where(t => t.PlayerId == userId);
                var myPlayEvents = await myTournamentPlayers.Select(t => t.Tournament).ExecuteAsync();

                foreach (Tournament myPlayEvent in myPlayEvents)
                {
	                myPlayEvent.Players = await _tournamentPlayerRepository.Query()
		                .Where(tp => tp.TournamentId == myPlayEvent.Id).ExecuteAsync();
                }
                
                List<TournamentListDisplayModel> upcomingEvents = new List<TournamentListDisplayModel>();
                List<TournamentListDisplayModel> previousEvents = new List<TournamentListDisplayModel>();

                List<Tournament> myEvents = new List<Tournament>();
                if (myTOEvents != null && myTOEvents.Any())
                {
	                myEvents.AddRange(myTOEvents);
                }

                if (myPlayEvents != null && myPlayEvents.Any())
                {
	                myEvents.AddRange(myPlayEvents);
                }

                myEvents = myEvents.Distinct().ToList();

				foreach (Tournament tournament in myEvents.Where(t => t.Date >= Date.FromDateTime(DateTime.Today)).Take(10))
				{
					TO = await _userManager.FindByIdAsync(tournament.TOId.ToString());
	                upcomingEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
		                tournament.Players, TO.UserName, tournament.Location(), TO.Id == userId));
                }

                foreach (Tournament tournament in myEvents.Where(t => t.Date < Date.FromDateTime(DateTime.Today)).Take(10))
                {
	                TO = await _userManager.FindByIdAsync(tournament.TOId.ToString());
	                previousEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
		                tournament.Players, TO.UserName, tournament.Location(), TO.Id == userId));
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
		            TO = await _userManager.FindByIdAsync(recentEvent.TOId.ToString());
		            tournaments.Add(new TournamentListDisplayModel(recentEvent.Id, recentEvent.Name, recentEvent.Date, recentEvent.Players, 
			            TO.UserName, recentEvent.Location(), false));
	            }
                return View("Index", tournaments);
            }
        }

        [HttpGet]
        public IActionResult MultiTest()
        {
	        MultiTestViewModel model = new MultiTestViewModel();
            model.Persons.Add(new Person(){FamilyName = "Solo", GivenName = "Han", Title = "Smugggler"});
            model.Persons.Add(new Person(){FamilyName = "Fel", GivenName = "Soontir", Title = "Baron"});
            model.Persons.Add(new Person() {FamilyName = "Fett", GivenName = "Boba", Title = "Bounty Hunter"});

            return View(model);
        }

        public IActionResult MultiTest(MultiTestViewModel model)
        {
	        if (ModelState.IsValid)
	        {

	        }

            return View(model);
        }
    }

    public class MultiTestViewModel
    {
	    public MultiTestViewModel()
	    {
		    Persons = new List<Person>();
	    }
	    public List<Person> Persons { get; set; }
    }

    public class Person
    {
	    public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public string Title { get; set; }
    }
}
