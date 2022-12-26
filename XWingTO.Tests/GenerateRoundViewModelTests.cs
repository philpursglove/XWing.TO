using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XWingTO.Core;
using XWingTO.Web.ViewModels.Tournament;

namespace XWingTO.Tests
{
	[TestFixture]
	public class GenerateRoundViewModelTests
	{
		private TournamentPlayer player1;
		private TournamentPlayer player2;
		private TournamentPlayer player3;

		private GenerateRoundViewModel model;

		[SetUp]
		public void Setup()
		{
			player1 = new TournamentPlayer() {Id = Guid.NewGuid()};
			player2 = new TournamentPlayer() {Id = Guid.NewGuid()};
			player3 = new TournamentPlayer() {Id = Guid.NewGuid()};

			model = new GenerateRoundViewModel();
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player1_List_Multiple_Times()
		{
			TournamentRound round = new TournamentRound();
			Game game1 = new Game
			{
				Player1 = player1,
				Player2 = player2
			};
			Game game2 = new Game
			{
				Player1 = player1,
				Player2 = player3
			};
			round.Games = new List<Game>
			{
				game1,
				game2
			};
			model.Round = round;

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player2_List_Multiple_Times()
		{
			TournamentRound round = new TournamentRound();
			Game game1 = new Game
			{
				Player1 = player1,
				Player2 = player2
			};
			Game game2 = new Game
			{
				Player1 = player3,
				Player2 = player2
			};
			round.Games = new List<Game>
			{
				game1,
				game2
			};
			model.Round = round;

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player1_List_And_PLayer2_List()
		{
			TournamentRound round = new TournamentRound();
			Game game1 = new Game
			{
				Player1 = player1,
				Player2 = player2
			};
			Game game2 = new Game
			{
				Player1 = player3,
				Player2 = player1
			};
			round.Games = new List<Game>
			{
				game1,
				game2
			};
			model.Round = round;

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);

		}

	}
}
