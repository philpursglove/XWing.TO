using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.ViewModels.Home;
using XWingTO.Web.ViewModels;
using XWingTO.Web.ViewModels.Tournament;

namespace XWingTO.Web.Controllers
{
	public class TournamentController : Controller
	{
		private readonly IRepository<Tournament, Guid> _tournamentRepository;
		private readonly IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		public TournamentController(IRepository<Tournament, Guid> tournamentRepository,
			IRepository<TournamentPlayer, Guid> tournamentPlayerRepository,
			UserManager<ApplicationUser> userManager)
		{
			_tournamentRepository = tournamentRepository;
			_tournamentPlayerRepository = tournamentPlayerRepository;
			_userManager = userManager;
		}

		[Authorize]
		public IActionResult Index()
		{
			return RedirectToAction("MyEvents");
		}

		public IActionResult Index(Guid id)
		{
			return RedirectToAction("Display", new { id });
		}

		public async Task<IActionResult> Display(Guid id)
		{
			Tournament tournament = await _tournamentRepository.Query().FirstOrDefault(t => t.Id == id);

			TournamentDisplayModel model = new TournamentDisplayModel
			{
				Tournament = tournament
			};

			var organiser = await _userManager.FindByIdAsync(tournament.TOId.ToString());
			model.TOName = organiser.UserName;

			var tournamentPlayers = await _tournamentPlayerRepository.Query().Where(tp => tp.TournamentId == tournament.Id).Include(p => p.Player).ExecuteAsync();
			if (tournamentPlayers.Any())
			{
				foreach (TournamentPlayer tournamentPlayer in tournamentPlayers)
				{
					model.Players.Add(new TournamentPlayerDisplayModel
					{ Points = tournamentPlayer.Points, Name = tournamentPlayer.Player.UserName });
				}

				if (HttpContext.User.Identity.IsAuthenticated)
				{
					var currentUser = await _userManager.GetUserAsync(HttpContext.User);
					if (tournamentPlayers.Any(tp => tp.PlayerId == currentUser.Id))
					{
						model.UserIsRegistered = true;
					}
					else
					{
						model.UserIsRegistered = false;
					}
				}

				model.PlayerCount = tournamentPlayers.Count().ToString();
			}
			else
			{
				model.PlayerCount = "0";
				model.UserIsRegistered = false;
			}

			return View(model);
		}

		public IActionResult Search()
		{
			return View();
		}

		[Authorize]
		public async Task<IActionResult> MyEvents()
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

			return View("MyEvents", model);

		}

		[Authorize]
		public async Task<IActionResult> Create()
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

			CreateTournamentViewModel model = new CreateTournamentViewModel()
			{
				Country = currentUser.Country,
				State = currentUser.State,
				City = currentUser.City,
				Venue = currentUser.Venue
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create(CreateTournamentViewModel model)
		{
			if (model.Date == new DateOnly(1, 1, 1))
			{
				model.Date = DateOnly.Parse(Request.Form["Date"]);
			}

			model.Validate(new ValidationContext(model));
			if (ModelState.IsValid)
			{
				ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
				Guid userId = currentUser.Id;

				Tournament tournament = new Tournament()
				{
					Name = model.Name,
					Date = model.Date,
					Country = model.Country,
					State = model.State,
					City = model.City,
					Venue = model.Venue,
					CreationDate = DateTime.Now,
					TOId = userId
				};

				await _tournamentRepository.Add(tournament);

				return RedirectToAction("Edit", "Tournament", new { tournament.Id });
			}

			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> Edit(Guid id)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

			Tournament tournament = await _tournamentRepository.Get(id);

			if (tournament.TOId == currentUser.Id)
			{
				EditTournamentViewModel model = new EditTournamentViewModel()
				{
					Id = id,
					Name = tournament.Name,
					Date = tournament.Date,
					Country = tournament.Country,
					State = tournament.State,
					City = tournament.City,
					Venue = tournament.Venue
				};
				return View(model);
			}
			else
			{
				return Forbid();
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Edit(EditTournamentViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

				Tournament tournament = await _tournamentRepository.Get(model.Id);

				if (tournament.TOId == currentUser.Id)
				{
					tournament.Name = model.Name;
					tournament.Date = model.Date;
					tournament.Country = model.Country;
					tournament.State = model.State;
					tournament.City = model.City;
					tournament.Venue = model.Venue;

					await _tournamentRepository.Update(tournament);

					return RedirectToAction("Edit", new {model.Id});
				}
				else
				{
					return Forbid();
				}
			}

			return View(model);

		}

		[Authorize]
		public async Task<IActionResult> Register(Guid tournamentId)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
			bool existingPlayer = _tournamentPlayerRepository.Query().Any(tp => tp.PlayerId == currentUser.Id);

			if (!existingPlayer)
			{
				TournamentPlayer tournamentPlayer = new TournamentPlayer
				{
					TournamentId = tournamentId,
					PlayerId = currentUser.Id,
				};
				await _tournamentPlayerRepository.Add(tournamentPlayer);
			}

			return RedirectToAction("Display", new {id = tournamentId});
		}

		[Authorize]
		public async Task<IActionResult> Unregister(Guid tournamentId)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
			TournamentPlayer tournamentPlayer = await _tournamentPlayerRepository.Query()
				.FirstOrDefault(tp => tp.PlayerId == currentUser.Id && tp.TournamentId == tournamentId);

			await _tournamentPlayerRepository.Delete(tournamentPlayer);

			return RedirectToAction("Display", new { id = tournamentId });
		}

		[Authorize]
		public async Task<IActionResult> GenerateRound(Guid id)
		{
			Tournament tournament = await _tournamentRepository.Query().Include(t => t.Players).Include(t => t.Rounds)
				.FirstOrDefault(t => t.Id == id);

			TournamentRound round = new TournamentRound
			{
				TournamentId = id,
				RoundNumber = tournament.Rounds.Count() + 1
			};

			IPairingStrategy strategy;
			if (tournament.Rounds.Any())
			{
				strategy = new PointsPairingStrategy();
			}
			else
			{
				strategy = new PointsPairingStrategy();
			}

			Pairer pairer = new Pairer(strategy);

			round.Games = pairer.Pair(tournament.Players.ToList());

			return View(new RoundViewModel{Round = round});
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> GenerateRound(RoundViewModel round)
		{
			return Ok();
		}
	}
}
