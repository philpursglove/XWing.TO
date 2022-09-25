using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.ViewModels.Tournament;

namespace XWingTO.Web.Controllers
{
	public class TournamentController : Controller
	{
		private readonly IRepository<Tournament, Guid> _tournamentRepository;
		private readonly IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		public TournamentController(IRepository<Tournament, Guid> tournamentRepository, IRepository<TournamentPlayer, Guid> tournamentPlayerRepository)
		{
			_tournamentRepository = tournamentRepository;
			_tournamentPlayerRepository = tournamentPlayerRepository;
		}

		public IActionResult Index()
		{
			return RedirectToAction("MyEvents");
		}

		public IActionResult Index(Guid id)
		{
			return RedirectToAction("Display", new {id});
		}

		public async Task<IActionResult> Display(Guid id)
		{
			Tournament tournament = await _tournamentRepository.Get(id);
			return View();
		}

		public IActionResult Search()
		{
			return View();
		}

		public async Task<IActionResult> MyEvents()
		{
			var myEvents = _tournamentRepository.Query();
			return View();
		}


		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(CreateTournamentViewModel model)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = new Tournament()
				{
					Name = model.Name,
					Date = model.Date,
					Country = model.Country,
					State = model.State,
					City = model.City,
					Venue = model.Venue
				};

				_tournamentRepository.Add(tournament);

				return RedirectToAction("Edit", "Tournament", new { tournament.Id});
			}

			return View(model);
		}

		public IActionResult Edit(Guid Id)
		{
			return View();
		}

		public async Task<IActionResult> Register(Guid id)
		{
			Tournament tournament = await _tournamentRepository.Get(id);
			RegisterViewModel model = new RegisterViewModel();
			return View(model);
		}

		[HttpPost]
		public IActionResult Register(RegisterViewModel model)
		{
			TournamentPlayer tournamentPlayer = new TournamentPlayer()
			{
				TournamentId = model.TournamentId,
				PlayerId = model.PlayerId
			};

			_tournamentPlayerRepository.Add(tournamentPlayer);

			return RedirectToAction("MyEvents");
		}
	}
}
