using System.ComponentModel.DataAnnotations;
using Azure.Storage.Queues;
using Humanizer;
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

		public IActionResult Index(Guid tournamentId)
		{
			return RedirectToAction("Display", new { tournamentId });
		}

		public async Task<IActionResult> Display(Guid tournamentId)
		{
			Tournament tournament = await _tournamentRepository.Query().FirstOrDefault(t => t.Id == tournamentId);
			List<TournamentRound> rounds = await _tournamentRoundRepository.Query()
				.Where(tr => tr.TournamentId == tournament.Id).Include(tr => tr.Games).ExecuteAsync();
			tournament.Rounds = rounds;

			TournamentDisplayModel model = new TournamentDisplayModel(tournament);
			model.Tournament = tournament;

			var organiser = await _userManager.FindByIdAsync(tournament.TOId.ToString());
			model.TOName = organiser.DisplayName;

			model.FormatName = tournament.Format.Humanize();

			var tournamentPlayers = await _tournamentPlayerRepository.Query().Where(tp => tp.TournamentId == tournament.Id).Include(p => p.Player).ExecuteAsync();
			if (tournamentPlayers.Any())
			{
				foreach (TournamentPlayer tournamentPlayer in tournamentPlayers)
				{
					model.Players.Add(new TournamentPlayerDisplayModel
					{ Points = tournamentPlayer.Points, Name = tournamentPlayer.Player.DisplayName, Dropped = tournamentPlayer.Dropped, 
						MissionPoints = tournamentPlayer.MissionPoints});
				}

				if (HttpContext.User.Identity!.IsAuthenticated)
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
			}
			else
			{
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
			if (model.StartDate == new Date(1, 1, 1))
			{
				switch (Request.Form["StartDate"].Count)
				{
					case 0:
						model.StartDate = Date.FromDateTime(DateTime.Today.AddMonths(-1));
						break;
					case > 1:
						model.StartDate = Date.Parse(Request.Form["StartDate"][0]);
						break;
					default:
						model.StartDate = Date.Parse(Request.Form["StartDate"]);
						break;
				}
			}
			if (model.EndDate == new Date(1, 1, 1))
			{
				switch (Request.Form["EndDate"].Count)
				{
					case 0:
						model.EndDate = Date.FromDateTime(DateTime.Today.AddMonths(1));
						break;
					case > 1:
						model.EndDate = Date.Parse(Request.Form["EndDate"][0]);
						break;
					default:
						model.EndDate = Date.Parse(Request.Form["EndDate"]);
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

			if (HttpContext.User.Identity!.IsAuthenticated)
			{
				ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
				Guid userId = user.Id;

				foreach (Tournament searchTournament in searchTournaments)
				{
					ApplicationUser TO = await _userManager.FindByIdAsync(searchTournament.TOId.ToString());
					modelList.Add(new TournamentListDisplayModel(searchTournament.Id, searchTournament.Name, searchTournament.Date,
						searchTournament.Players, TO.DisplayName, searchTournament.Location(), searchTournament.TOId == userId, 
						searchTournament.Format.Humanize()));
				}
			}
			else
			{
				foreach (Tournament searchTournament in searchTournaments)
				{
					ApplicationUser TO = await _userManager.FindByIdAsync(searchTournament.TOId.ToString());
					modelList.Add(new TournamentListDisplayModel(searchTournament.Id, searchTournament.Name, searchTournament.Date,
						searchTournament.Players, TO.DisplayName, searchTournament.Location(), false, searchTournament.Format.Humanize()));
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

			var myEvents = myTOEvents.Union(myPlayEvents);

			ApplicationUser TO;

			foreach (Tournament tournament in myEvents.Where(t => t.Date >= Date.FromDateTime(DateTime.Today)).Take(10))
			{
				TO = await _userManager.FindByIdAsync(tournament.TOId.ToString());
				upcomingEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
					tournament.Players, TO.DisplayName , tournament.Location(), TO.Id == userId, tournament.Format.Humanize()));
			}

			foreach (Tournament tournament in myEvents.Where(t => t.Date < Date.FromDateTime(DateTime.Today)).Take(10))
			{
				TO = await _userManager.FindByIdAsync(tournament.TOId.ToString());
				previousEvents.Add(new TournamentListDisplayModel(tournament.Id, tournament.Name, tournament.Date,
					tournament.Players, TO.DisplayName, tournament.Location(), TO.Id == userId, tournament.Format.Humanize()));
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
			if (model.Date == new Date(1, 1, 1))
			{
				DateTime modelDate;
				bool dateIsPresent = DateTime.TryParse(Request.Form["Date"], out modelDate);
				if (dateIsPresent)
				{
					ModelState.SetModelValue("Date", modelDate, Request.Form["Date"]);
					model.Date = Date.FromDateTime(modelDate);
					ModelState.ClearValidationState("Date");
					var validationResult = model.Validate(new ValidationContext(model));
					if (!validationResult.Any())
					{
						ModelState.MarkFieldValid("Date");
					}
				}
				else
				{
					model.Date = new Date(1,1,1);
				}
			}

			
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
					TOId = userId,
					Format = model.Format
				};

				await _tournamentRepository.Add(tournament);

				return RedirectToAction("Admin", "Tournament", new { tournamentId = tournament.Id });
			}

			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> Admin(Guid tournamentId)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

			Tournament tournament = await _tournamentRepository.Get(tournamentId);

			if (tournament.TOId == currentUser.Id)
			{
				TournamentAdminModel model = new TournamentAdminModel()
				{
					Id = tournamentId,
					Name = tournament.Name,
					Date = tournament.Date,
					Country = tournament.Country,
					State = tournament.State,
					City = tournament.City,
					Venue = tournament.Venue,
					Format = tournament.Format
				};

				model.Players = new List<TournamentPlayerDisplayModel>();
				List<TournamentPlayer> tournamentPlayers = await _tournamentPlayerRepository.Query()
					.Where(tp => tp.TournamentId == tournament.Id).ExecuteAsync();
				foreach (TournamentPlayer tournamentPlayer in tournamentPlayers)
				{
					ApplicationUser playerUser = await _userManager.FindByIdAsync(tournamentPlayer.PlayerId.ToString());
					model.Players.Add(new TournamentPlayerDisplayModel(){Name = playerUser.DisplayName, TournamentPlayerId = tournamentPlayer.Id, 
						Points = tournamentPlayer.Points, Dropped = tournamentPlayer.Dropped, MissionPoints = tournamentPlayer.MissionPoints});
				}

				if (tournamentPlayers.Select(tp => tp.PlayerId).Contains(currentUser.Id))
				{
					model.UserIsRegistered = true;
				}

				model.Rounds = new List<TournamentRoundDisplayModel>();
				List<TournamentRound> tournamentRounds = await _tournamentRoundRepository.Query()
					.Where(tr => tr.TournamentId == tournamentId).ExecuteAsync();
				foreach (TournamentRound tournamentRound in tournamentRounds)
				{
					TournamentRoundDisplayModel roundDisplayModel = new TournamentRoundDisplayModel();
					roundDisplayModel.Round = tournamentRound.RoundNumber;

					roundDisplayModel.Games = new List<TournamentGameDisplayModel>();
					List<Game> roundGames = await _gameRepository.Query().Where(g => g.TournamentRoundId == tournamentRound.Id).ExecuteAsync();
					foreach (Game roundGame in roundGames)
					{
						TournamentPlayer player1 = await _tournamentPlayerRepository.Query().Include(p => p.Player)
							.FirstOrDefault(tp => tp.Id == roundGame.TournamentPlayer1Id);
						TournamentPlayer player2 = await _tournamentPlayerRepository.Query().Include(p => p.Player).FirstOrDefault(tp => tp.Id == roundGame.TournamentPlayer2Id);
						roundDisplayModel.Games.Add(new TournamentGameDisplayModel(){GameId = roundGame.Id, Player1 = player1.Player.DisplayName, 
							Player2 = player2.Player.DisplayName, Player1Score = roundGame.Player1MissionPoints, Player2Score = roundGame.Player2MissionPoints,	
							Turns = roundGame.Turns
						});
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
		public async Task<IActionResult> Cancel(Guid tournamentId)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
			Tournament tournament = await _tournamentRepository.Get(tournamentId);

			if (currentUser.Id == tournament.TOId)
			{
				await _tournamentRepository.Delete(tournament);
				return RedirectToAction("Index", "Home");
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
			if (model.Date == new Date(1, 1, 1))
			{
				model.Date = Date.Parse(Request.Form["Date"]);
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
					tournament.Format = model.Format;

					await _tournamentRepository.Update(tournament);

					return RedirectToAction("Admin", new { tournamentId = model.Id });
				}
				else
				{
					return Forbid();
				}
			}

			return RedirectToAction("Admin", new {tournamentId = model.Id});

		}

		[Authorize]
		public async Task<IActionResult> Register(Guid tournamentId)
		{
			ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
			List<TournamentRound> rounds =
				await _tournamentRoundRepository.Query().Where(t => t.TournamentId == tournamentId).ExecuteAsync();
			if (!rounds.Any())
			{
				bool existingPlayer = _tournamentPlayerRepository.Query().Any(tp => tp.PlayerId == currentUser.Id && tp.TournamentId == tournamentId);

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

			Tournament tournament = await _tournamentRepository.Get(tournamentId);
			if (tournament.TOId == currentUser.Id)
			{
				return RedirectToAction("Admin", new {tournamentId});
			}
			else
			{
				return RedirectToAction("Display", new {tournamentId });
			}
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
                return RedirectToAction("Admin", new {tournamentId});
            }
            else
            {
                return RedirectToAction("Display", new {tournamentId });
            }
        }

		[Authorize]
		public async Task<IActionResult> GenerateRound(Guid tournamentId, RoundGenerationMode mode = RoundGenerationMode.Automatic)
		{
			Tournament tournament = await _tournamentRepository.Query().Include(t => t.Players).Include(t => t.Rounds)
				.FirstOrDefault(t => t.Id == tournamentId);

			switch (mode)
			{
				case RoundGenerationMode.Manual:
					return await ManualGenerateRound(tournament);
				case RoundGenerationMode.Automatic:
					return await AutoGenerateRound(tournament);
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}

		}

		private async Task<IActionResult> AutoGenerateRound(Tournament tournament)
		{
			TournamentRound round = new TournamentRound() {Id = Guid.NewGuid(), TournamentId = tournament.Id};

			IPairingStrategy strategy;
			if (tournament.Rounds.Any())
			{
				round.RoundNumber = tournament.Rounds.Count() + 1;
				strategy = new PointsPairingStrategy();
			}
			else
			{
				round.RoundNumber = 1;
				strategy = new RandomPairingStrategy();
			}

			Pairer pairer = new Pairer(strategy);

			List<Game> pairedGames = pairer.Pair(tournament.Players.ToList());

			round.Games = pairedGames;

			await _tournamentRoundRepository.Add(round);

			return RedirectToAction("Display", new {tournamentId = tournament.Id});
		}

		private async Task<IActionResult> ManualGenerateRound(Tournament tournament)
		{
			GenerateRoundRoundViewModel round = new GenerateRoundRoundViewModel
			{
				TournamentId = tournament.Id,
				Id = Guid.NewGuid()
			};

			IPairingStrategy strategy;
			if (tournament.Rounds.Any())
			{
				round.RoundNumber = tournament.Rounds.Count() + 1;
				strategy = new PointsPairingStrategy();
			}
			else
			{
				round.RoundNumber = 1;
				strategy = new RandomPairingStrategy();
			}

			Pairer pairer = new Pairer(strategy);

			List<Game> pairedGames = pairer.Pair(tournament.Players.ToList());

			foreach (Game pairedGame in pairedGames)
			{
				round.Games.Add(new GenerateRoundGameViewModel(pairedGame));
			}

			GenerateRoundViewModel model = new GenerateRoundViewModel(
				await _tournamentPlayerRepository.Query().Include(p => p.Player)
					.Where(tp => tp.TournamentId == tournament.Id).ExecuteAsync(), round);

			return View(model);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> GenerateRound(GenerateRoundViewModel model)
		{
			model.Round.Games = model.Games;

			ModelState.ClearValidationState(nameof(model));
			TryValidateModel(model);
			model.Validate(new ValidationContext(model));

			if (ModelState.IsValid)
			{
				TournamentRound round = new TournamentRound { Id = model.Round.Id, TournamentId = model.Round.TournamentId, RoundNumber = model.Round.RoundNumber };
				List<Game> games = new List<Game>();
				foreach (GenerateRoundGameViewModel gameViewModel in model.Games)
				{
					Game game = new Game()
					{
						Id = gameViewModel.Id,
						TournamentRoundId = gameViewModel.TournamentRoundId,
						TableNumber = gameViewModel.TableNumber,
						TournamentPlayer1Id = gameViewModel.TournamentPlayer1Id,
						TournamentPlayer2Id = gameViewModel.TournamentPlayer2Id
					};
					games.Add(game);
				}
				round.Games = games;
				await _tournamentRoundRepository.Add(round);

				return RedirectToAction("Admin", new {tournamentId = model.Round.TournamentId});
			}

			return View(model);
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
				model.Player1Name = user1.DisplayName;
				model.Player2Name = user2.DisplayName;
				model.GameId = game.Id;
				model.TournamentId = tournamentId;
				model.Player1MissionPoints = game.Player1MissionPoints;
				model.Player2MissionPoints = game.Player2MissionPoints;

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
				QueueClient queueClient = new QueueClient(queueConnectionString, "gamescorequeue", new QueueClientOptions
				{
					MessageEncoding = QueueMessageEncoding.Base64
				});

				await queueClient.CreateIfNotExistsAsync();

				if (await queueClient.ExistsAsync())
				{
					await queueClient.SendMessageAsync(SerializeObject(scoreMessage));
				}

				return RedirectToAction("Display", new { tournamentId = model.TournamentId });
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Drop(Guid tournamentPlayerId, bool dropped, Guid tournamentId)
		{
			TournamentPlayer player = await _tournamentPlayerRepository.Get(tournamentPlayerId);

			player.Dropped = dropped;

			await _tournamentPlayerRepository.Update(player);

			return RedirectToAction("Admin", new {tournamentId});
		}
	}
}
