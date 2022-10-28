using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using XWingTO.Core;
using XWingTO.Core.Messages;
using XWingTO.Data;
using XWingTO.Functions;

namespace XWingTO.Tests.Functions
{
	[TestFixture]
	public class ProcessGameScoreTests
	{
		private Guid _gameId = Guid.Parse("B4A6EABC-EC67-4CE3-BA2B-688CC3B92463");
		private Guid _player1Id = Guid.Parse("3E94A69B-505C-4288-AC44-2253D1D0045C");
		private Guid _player2Id = Guid.Parse("583D73A8-774E-4929-B259-49A36A4CCD0B");
		private IRepository<Game, Guid> _gameRepository;
		private IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		private ILogger _logger;

		[SetUp]
		public void Setup()
		{
			Game game = new Game {Id = _gameId, TournamentPlayer1Id = _player1Id, TournamentPlayer2Id = _player2Id};
			_gameRepository = Substitute.For<IRepository<Game, Guid>>();
			_gameRepository.Get(_gameId).Returns(game);

			TournamentPlayer player1 = new TournamentPlayer {Id = _player1Id};
			TournamentPlayer player2 = new TournamentPlayer {Id = _player2Id};
			_tournamentPlayerRepository = Substitute.For<IRepository<TournamentPlayer, Guid>>();
			_tournamentPlayerRepository.Get(_player1Id).Returns(player1);
			_tournamentPlayerRepository.Get(_player2Id).Returns(player2);

			_logger = Substitute.For<ILogger>();
		}

		[Test]
		public async Task When_Player_1_Wins_Player_1_Gets_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage {GameId = _gameId, Player1Points = 20, Player2Points = 10};

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(_player1Id);
			Assert.That(player1.Points, Is.EqualTo(3));
		}

		[Test]
		public async Task When_Player_2_Wins_Player_2_Gets_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 20 };

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(_player2Id);
			Assert.That(player2.Points, Is.EqualTo(3));
		}

		[Test]
		public async Task When_Game_Is_A_Draw_Both_Players_Receive_One_Point()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10 };

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(_player1Id);
			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(_player2Id);
			Assert.Multiple(() =>
			{
				Assert.That(player1.Points, Is.EqualTo(1));
				Assert.That(player2.Points, Is.EqualTo(1));
			});
		}

		[Test]
		public async Task When_Player_1_Concedes_Player_2_Receives_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10, Player1Concede = true};

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(_player2Id);
			Assert.That(player2.Points, Is.EqualTo(3));
		}

		[Test]
		public async Task When_Player_2_Concedes_Player_1_Receives_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10, Player2Concede = true };

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(_player1Id);
			Assert.That(player1.Points, Is.EqualTo(3));
		}

		[Test]
		public async Task When_Player_1_Drops_TournamentPlayer_Is_Updated()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10, Player1Drop = true};

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(_player1Id);
			Assert.That(player1.Dropped, Is.True);
		}

		[Test]
		public async Task When_Player_2_Drops_TournamentPlayer_Is_Updated()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10, Player2Drop = true };

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(_player2Id);
			Assert.That(player2.Dropped, Is.True);
		}

		[Test]
		public async Task When_Both_Players_Drop_Both_TournamentPlayers_Are_Updated()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = _gameId, Player1Points = 10, Player2Points = 10, Player1Drop = true, Player2Drop = true};

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(_player1Id);
			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(_player2Id);
			Assert.Multiple(() =>
			{
				Assert.That(player1.Dropped, Is.True);
				Assert.That(player2.Dropped, Is.True);
			});
		}
	}
}
