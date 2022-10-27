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
		private Guid gameId = Guid.Parse("B4A6EABC-EC67-4CE3-BA2B-688CC3B92463");
		private Guid player1Id = Guid.Parse("3E94A69B-505C-4288-AC44-2253D1D0045C");
		private Guid player2Id = Guid.Parse("583D73A8-774E-4929-B259-49A36A4CCD0B");
		private IRepository<Game, Guid> _gameRepository;
		private IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		private ILogger _logger;

		[SetUp]
		public void Setup()
		{
			Game game = new Game {Id = gameId, TournamentPlayer1Id = player1Id, TournamentPlayer2Id = player2Id};
			_gameRepository = Substitute.For<IRepository<Game, Guid>>();
			_gameRepository.Get(gameId).Returns(game);

			TournamentPlayer player1 = new TournamentPlayer {Id = player1Id};
			TournamentPlayer player2 = new TournamentPlayer {Id = player2Id};
			_tournamentPlayerRepository = Substitute.For<IRepository<TournamentPlayer, Guid>>();
			_tournamentPlayerRepository.Get(player1Id).Returns(player1);
			_tournamentPlayerRepository.Get(player2Id).Returns(player2);

			_logger = Substitute.For<ILogger>();
		}

		[Test]
		public async Task When_Player_1_Wins_Player_1_Gets_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage {GameId = gameId, Player1Points = 20, Player2Points = 10};

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player1 = await _tournamentPlayerRepository.Get(player1Id);
			Assert.That(player1.Points, Is.EqualTo(3));
		}

		[Test]
		public async Task When_Player_2_Wins_Player_2_Gets_Three_Points()
		{
			GameScoreMessage message = new GameScoreMessage { GameId = gameId, Player1Points = 10, Player2Points = 20 };

			ProcessGameScore function = new ProcessGameScore(_gameRepository, _tournamentPlayerRepository);
			await function.Run(JsonConvert.SerializeObject(message), _logger);

			TournamentPlayer player2 = await _tournamentPlayerRepository.Get(player2Id);
			Assert.That(player2.Points, Is.EqualTo(3));
		}

	}
}
