using System.ComponentModel.DataAnnotations;
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

		private List<TournamentPlayer> PlayerList;

		private GenerateRoundViewModel model;

		[SetUp]
		public void Setup()
		{
			player1 = new TournamentPlayer
			{
				Id = Guid.NewGuid(),
				Player = new ApplicationUser() {DisplayName = "Alice"}
			};
			player2 = new TournamentPlayer()
				{Id = Guid.NewGuid(), Player = new ApplicationUser() {DisplayName = "Bob"}};
			player3 = new TournamentPlayer() {Id = Guid.NewGuid(), Player = new ApplicationUser(){DisplayName = "Charlie"}};

			PlayerList = new List<TournamentPlayer>
			{
				player1, player2, player3
			};
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player1_List_Multiple_Times()
		{
			GenerateRoundRoundViewModel round = new GenerateRoundRoundViewModel();
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
			round.Games = new List<GenerateRoundGameViewModel>
			{
				new(game1),
				new(game2)
			};

			model = new GenerateRoundViewModel(PlayerList, round);

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player2_List_Multiple_Times()
		{
			GenerateRoundRoundViewModel round = new GenerateRoundRoundViewModel();
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
			round.Games = new List<GenerateRoundGameViewModel>
			{
				new(game1),
				new(game2)
			};
			model = new GenerateRoundViewModel(PlayerList, round);

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);
		}

		[Test]
		public void A_Player_Should_Not_Appear_In_Player1_List_And_PLayer2_List()
		{
			GenerateRoundRoundViewModel round = new GenerateRoundRoundViewModel();
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
			round.Games = new List<GenerateRoundGameViewModel>
			{
				new(game1),
				new(game2)
			};
			model = new GenerateRoundViewModel(PlayerList, round);

			IEnumerable<ValidationResult> results = model.Validate(new ValidationContext(model));
			Assert.That(results.Any(), Is.True);
		}
	}
}
