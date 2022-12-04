﻿using System.ComponentModel.DataAnnotations;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Core.Messages;
using XWingTO.Data;
using XWingTO.Web.ViewModels.Home;
using XWingTO.Web.ViewModels;
using XWingTO.Web.ViewModels.Tournament;
using static Newtonsoft.Json.JsonConvert;

namespace XWingTO.Web.Controllers
{
	public class TournamentController : Controller
	{
		private readonly IRepository<Tournament, Guid> _tournamentRepository;
		private readonly IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IRepository<Game, Guid> _gameRepository;
		private readonly IConfiguration _configuration;
		private readonly IRepository<TournamentRound, Guid> _tournamentRoundRepository;
		public TournamentController(IRepository<Tournament, Guid> tournamentRepository,
			IRepository<TournamentPlayer, Guid> tournamentPlayerRepository,
			UserManager<ApplicationUser> userManager,
			IRepository<Game, Guid> gameRepository,
			IConfiguration configuration,
			IRepository<TournamentRound, Guid> tournamentRoundRepository)
		{
			_tournamentRepository = tournamentRepository;
			_tournamentPlayerRepository = tournamentPlayerRepository;
			_userManager = userManager;
			_gameRepository = gameRepository;
			_configuration = configuration;
			_tournamentRoundRepository = tournamentRoundRepository;
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
			List<TournamentRound> rounds = await _tournamentRoundRepository.Query()
				.Where(tr => tr.TournamentId == tournament.Id).Include(tr => tr.Games).ExecuteAsync();
			tournament.Rounds = rounds;

			TournamentDisplayModel model = new TournamentDisplayModel(tournament);
			model.Tournament = tournament;

			var organiser = await _userManager.FindByIdAsync(tournament.TOId.ToString());
			model.TOName = organiser.UserName;

			var tournamentPlayers = await _tournamentPlayerRepository.Query().Where(tp => tp.TournamentId == tournament.Id).Include(p => p.Player).ExecuteAsync();
			if (tournamentPlayers.Any())
			{
				foreach (TournamentPlayer tournamentPlayer in tournamentPlayers)
				{
					model.Players.Add(new TournamentPlayerDisplayModel
					{ Points = tournamentPlayer.Points, Name = tournamentPlayer.Player.UserName, Dropped = tournamentPlayer.Dropped});
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
			return View(new SearchViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Search(SearchViewModel model)
		{
			if (model.StartDate == new DateOnly(1, 1, 1))
			{
				switch (Request.Form["StartDate"].Count)
				{
					case 0:
						model.StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
						break;
					case > 1:
						model.StartDate = DateOnly.Parse(Request.Form["StartDate"][0]);
						break;
					default:
						model.StartDate = DateOnly.Parse(Request.Form["StartDate"]);
						break;
				}
			}
			if (model.EndDate == new DateOnly(1, 1, 1))
			{
				switch (Request.Form["EndDate"].Count)
				{
					case 0:
						model.EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
						break;
					case > 1:
						model.EndDate = DateOnly.Parse(Request.Form["EndDate"][0]);
						break;
					default:
						model.EndDate = DateOnly.Parse(Request.Form["EndDate"]);
						break;
				}
			}

			IQuery<Tournament> query = _tournamentRepository.Query();
			if (!string.IsNullOrWhiteSpace(model.Name))
			{
				query = query.Where(t => t.Name == model.Name);
			}

			query = query.Where(t => t.Date >= model.StartDate);
			query = query.Where(t => t.Date <= model.EndDate);

			List<Tournament> searchTournaments = await query.Include(t => t.Players).ExecuteAsync();

			List<TournamentListDisplayModel> modelList = new List<TournamentListDisplayModel>();

			if (HttpContext.User.Identity.IsAuthenticated)
			{
				ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
				Guid userId = user.Id;

				foreach (Tournament searchTournament in searchTournaments)
				{
					ApplicationUser TO = await _userManager.FindByIdAsync(searchTournament.TOId.ToString());
					modelList.Add(new TournamentListDisplayModel(searchTournament.Id, searchTournament.Name, searchTournament.Date,
						searchTournament.Players, TO.UserName, searchTournament.Location(), searchTournament.TOId == userId));
				}
			}
			else
			{
				foreach (Tournament searchTournament in searchTournaments)
				{
					ApplicationUser TO = await _userManager.FindByIdAsync(searchTournament.TOId.ToString());
					modelList.Add(new TournamentListDisplayModel(searchTournament.Id, searchTournament.Name, searchTournament.Date,
						searchTournament.Players, TO.UserName, searchTournament.Location(), false));
				}

			}

			model.Tournaments = modelList;

			ModelState.Clear();

			return View(model);
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

			ApplicationUser TO;

			foreach (Tournament tournament in myEvents.Where(t => t.Date >= DateOnly.FromDateTime(DateTime.Today)).Take(10))
			{
				TO = await _userManager.FindByIdAsync(tournament.TOId.ToString());
				upcomingEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
					tournament.Players, TO.UserName , tournament.Location(), TO.Id == userId));
			}

			foreach (Tournament tournament in myEvents.Where(t => t.Date < DateOnly.FromDateTime(DateTime.Today)).Take(10))
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

				return RedirectToAction("Admin", "Tournament", new { tournament.Id });
			}

			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> Admin(Guid id)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

			Tournament tournament = await _tournamentRepository.Get(id);

			if (tournament.TOId == currentUser.Id)
			{
				TournamentAdminModel model = new TournamentAdminModel()
				{
					Id = id,
					Name = tournament.Name,
					Date = tournament.Date,
					Country = tournament.Country,
					State = tournament.State,
					City = tournament.City,
					Venue = tournament.Venue
				};

				model.Players = new List<TournamentPlayerDisplayModel>();
				List<TournamentPlayer> tournamentPlayers = await _tournamentPlayerRepository.Query()
					.Where(tp => tp.TournamentId == tournament.Id).ExecuteAsync();
				foreach (TournamentPlayer tournamentPlayer in tournamentPlayers)
				{
					ApplicationUser playerUser = await _userManager.FindByIdAsync(tournamentPlayer.PlayerId.ToString());
					model.Players.Add(new TournamentPlayerDisplayModel(){Name = playerUser.UserName, TournamentPlayerId = tournamentPlayer.Id, 
						Points = tournamentPlayer.Points, Dropped = tournamentPlayer.Dropped});
				}

				model.Rounds = new List<TournamentRoundDisplayModel>();
				List<TournamentRound> tournamentRounds = await _tournamentRoundRepository.Query()
					.Where(tr => tr.TournamentId == id).ExecuteAsync();
				foreach (TournamentRound tournamentRound in tournamentRounds)
				{
					TournamentRoundDisplayModel roundDisplayModel = new TournamentRoundDisplayModel();
					roundDisplayModel.Round = tournamentRound.RoundNumber;

					roundDisplayModel.Games = new List<TournamentGameDisplayModel>();
					List<Game> roundGames = await _gameRepository.Query().Where(g => g.TournamentRoundId == tournamentRound.Id).ExecuteAsync();
					foreach (Game roundGame in roundGames)
					{
						ApplicationUser player1 = await _userManager.FindByIdAsync(roundGame.TournamentPlayer1Id.ToString());
						ApplicationUser player2 = await _userManager.FindByIdAsync(roundGame.TournamentPlayer2Id.ToString());
						roundDisplayModel.Games.Add(new TournamentGameDisplayModel(){GameId = roundGame.Id, Player1 = player1.UserName, 
							Player2 = player2.UserName, Player1Score = roundGame.Player1MissionPoints, Player2Score = roundGame.Player2MissionPoints});
					}

					model.Rounds.Add(roundDisplayModel);
				}

				return View(model);
			}
			else
			{
				return Forbid();
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Admin(TournamentAdminUpdateModel model)
		{
			if (model.Date == new DateOnly(1, 1, 1))
			{
				model.Date = DateOnly.Parse(Request.Form["Date"]);
			}

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

					return RedirectToAction("Admin", new { model.Id });
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
			List<TournamentRound> rounds =
				await _tournamentRoundRepository.Query().Where(t => t.TournamentId == tournamentId).ExecuteAsync();
			if (!rounds.Any())
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

			}

			return RedirectToAction("Display", new { id = tournamentId });
		}

		[Authorize]
		public async Task<IActionResult> Unregister(Guid tournamentId, Guid? tournamentPlayerId)
		{
			TournamentPlayer tournamentPlayer;
			if (tournamentPlayerId == null)
			{
				ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
				tournamentPlayer = await _tournamentPlayerRepository.Query()
					.FirstOrDefault(tp => tp.PlayerId == currentUser.Id && tp.TournamentId == tournamentId);
			}
            else
			{
				tournamentPlayer = await _tournamentPlayerRepository.Get(tournamentPlayerId.Value);
			}

			if (tournamentPlayer != null)
			{
				await _tournamentPlayerRepository.Delete(tournamentPlayer);
			}

            if (tournamentPlayerId.HasValue)
            {
                return RedirectToAction("Admin", new {id = tournamentId});
            }
            else
            {
                return RedirectToAction("Display", new { id = tournamentId });
            }
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
				strategy = new RandomPairingStrategy();
			}

			Pairer pairer = new Pairer(strategy);

			round.Games = pairer.Pair(tournament.Players.ToList());

			await _tournamentRoundRepository.Add(round);

			return RedirectToAction("Display", new { id = tournament.Id });
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> GenerateRound(RoundViewModel round)
		{
			return Ok(Task.CompletedTask);
		}

		[Authorize]
		public async Task<IActionResult> ResultSubmission(Guid gameId)
		{
			Game game = await _gameRepository.Query().Include(g => g.Round).FirstOrDefault(g => g.Id == gameId);
			if (game == null)
			{
				return NotFound();
			}

			Guid tournamentId = game.Round.TournamentId;
			Guid toId = _tournamentRepository.Query().FirstOrDefault(t => t.Id == tournamentId).Result.TOId;

			ResultSubmissionViewModel model = new ResultSubmissionViewModel();

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(game.TournamentPlayer1Id);
			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(game.TournamentPlayer2Id);
			ApplicationUser user1 = await _userManager.FindByIdAsync(player1.PlayerId.ToString());
			ApplicationUser user2 = await _userManager.FindByIdAsync(player2.PlayerId.ToString());

			Guid currentUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
			if (currentUserId == toId || currentUserId == game.TournamentPlayer1Id ||
				currentUserId == game.TournamentPlayer2Id)
			{
				model.Player1Name = user1.UserName;
				model.Player2Name = user2.UserName;
				model.GameId = game.Id;
				model.TournamentId = tournamentId;

				return View(model);
			}

			return Forbid();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ResultSubmission(ResultSubmissionViewModel model)
		{
			if (ModelState.IsValid)
			{
				GameScoreMessage scoreMessage = new GameScoreMessage
				{
					GameId = model.GameId,
					Player1Points = model.Player1MissionPoints,
					Player2Points = model.Player2MissionPoints,
					Turns = model.Turns,
					OutOfTime = model.OutOfTime,
					Player1Concede = model.Player1Concede,
					Player2Concede = model.Player2Concede,
					Player1Drop = model.Player1Drop,
					Player2Drop = model.Player2Drop
				};

				string queueConnectionString = _configuration.GetConnectionString("XWingTO.Queue");
				QueueClient queueClient = new QueueClient(queueConnectionString, "GameScoreQueue");

				await queueClient.CreateIfNotExistsAsync();

				if (await queueClient.ExistsAsync())
				{
					await queueClient.SendMessageAsync(SerializeObject(scoreMessage));
				}

				return RedirectToAction("Display", new { id = model.TournamentId });
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Drop(Guid tournamentPlayerId, bool dropped, Guid tournamentId)
		{
			TournamentPlayer player = await _tournamentPlayerRepository.Get(tournamentPlayerId);

			player.Dropped = dropped;

			await _tournamentPlayerRepository.Update(player);

			return RedirectToAction("Admin", new {id = tournamentId});
		}
	}
}
